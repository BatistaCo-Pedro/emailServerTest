using App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Events;
using App.Server.Notification.Infrastructure.Messaging.DomainEvents.Consumers;

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
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                optionsBuilder =>
                {
                    optionsBuilder
                        .EnableRetryOnFailure()
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            )
        );

        services.AddScoped<IUnitOfWork, NotificationDbContext>(sp =>
            sp.GetRequiredService<NotificationDbContext>()
        );

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

        // Add hangfire related services such as a queue service

        return services;
    }

    /// <summary>
    /// Adds messaging to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Configuration object with the messaging configuration.</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="ApplicationException">Messaging settings not found in configuration object.</exception>
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var messagingSettings =
            configuration.GetSection("MessagingSettings").Get<MessagingSettings>()
            ?? throw new ApplicationException("Messaging settings not found");

        services.AddMediator(config =>
        {
            config.AddConsumer<BaseConsumer<TestEvent>>();
        });

        // MassTransit
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
}
