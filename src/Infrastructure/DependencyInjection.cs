namespace App.Server.Notification.Infrastructure;

/// <summary>
/// Extension class containing dependency injection methods.
/// </summary>
public static class DependencyInjection
{
    // Add options here from the config file if they need to be in the container
    // public static IServiceCollection AddValidatedOptions<TOptions>()

    /// <summary>
    /// Adds the database to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string the database connects to.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddScoped<IDomainEventDispatcher, MassTransitDomainEventDispatcher>();

        services.AddDbContext<NotificationDbContext>(options =>
            options
                .UseLazyLoadingProxies()
                .UseNpgsql(
                    connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder
                            .EnableRetryOnFailure()
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                )
        );

        services.AddScoped<IUnitOfWork, NotificationDbContext>();

        return services;
    }

    /// <summary>
    /// Adds a background queue to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string the queue connects to.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBackgroundQueue(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddHangfire(config =>
            config
                .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString))
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
        );

        services.AddHangfireServer();

        services.AddScoped<IMailingQueue, HangfireQueue>();

        return services;
    }

    /// <summary>
    /// Adds messaging to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Configuration object with the messaging configuration.</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="ApplicationException">Messaging settings not found in configuration object.</exception>
    /// <remarks>
    /// This method adds consumers based on the event handlers registered in the application layer.
    /// </remarks>
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var messagingSettings =
            configuration.GetSection(nameof(MessagingSettings)).Get<MessagingSettings>()
            ?? throw new ApplicationException("Messaging settings not found");

        // Domain events mediator
        services.AddMediator(config => RegisterDomainConsumers(config, services));

        services.AddMassTransit(busConfiguration =>
        {
            //configureBus(busConfiguration);

            busConfiguration.UsingRabbitMq(
                (context, config) =>
                {
                    config.Host(
                        messagingSettings.Host,
                        messagingSettings.Port,
                        messagingSettings.VirtualHost,
                        host =>
                        {
                            host.Username(messagingSettings.Username);
                            host.Password(messagingSettings.Password);

                            if (messagingSettings.SslActive)
                            {
                                host.UseSsl(ssl =>
                                {
                                    ssl.Protocol = SslProtocols.None;
                                });
                            }
                        }
                    );
                }
            );

            busConfiguration.AddConfigureEndpointsCallback((name, config) => { });
        });

        return services;
    }

    /// <summary>
    /// Registers all consumers for domain events.
    /// </summary>
    /// <param name="configurator">The <see cref="MassTransit"/> <see cref="IMediatorRegistrationConfigurator"/>.</param>
    /// <param name="serviceCollection">The service collection to extend.</param>
    public static void RegisterDomainConsumers(
        IMediatorRegistrationConfigurator configurator,
        IServiceCollection serviceCollection
    )
    {
        var types = serviceCollection.Select(x => x.ImplementationType).OfType<Type>().ToArray();

        foreach (var eventHandlerType in TypeHelper.GetEventHandlerTypes(types))
        {
            if (!TypeHelper.IsDomainEvent(eventHandlerType.Key))
            {
                continue;
            }

            var consumerType = typeof(Consumer<>).MakeGenericType(eventHandlerType.Key);

            configurator.AddConsumer(consumerType);
        }
    }
}
