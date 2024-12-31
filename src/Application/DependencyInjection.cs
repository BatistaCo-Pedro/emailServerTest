namespace App.Server.Notification.Application;

/// <summary>
/// Extension class containing dependency injection methods for the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds services belonging to the application layer.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="configuration">The configuration from app settings.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddValidatedOptions<SmtpSettings>(configuration);

        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IDataOwnerService, DataOwnerService>();
        services.AddScoped<ITemplateTypeService, TemplateTypeService>();
        services.AddScoped<IMailingService, MailingService>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }

    /// <summary>
    /// Adds cryptographic services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="configuration">The configuration from app settings.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCryptographicServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddValidatedOptions<CryptographicSettings>(configuration);

        services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();
        services.AddScoped<AesEncryptionService>();

        AesEncryptionHelper.SetEncryptionService(
            services
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider.GetRequiredService<AesEncryptionService>()
        );

        return services;
    }

    /// <summary>
    /// Adds event handlers to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to extend.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEventHandlers(this IServiceCollection serviceCollection)
    {
        RegisterEventHandlers(serviceCollection);
        return serviceCollection;
    }

    /// <summary>
    /// Registers all consumers in the service collection based on the event handler registered.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    private static void RegisterEventHandlers(IServiceCollection serviceCollection)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (var eventHandlerType in TypeHelper.GetEventHandlerTypes(types))
        {
            serviceCollection.AddScoped(
                typeof(IEventHandler<>).MakeGenericType(eventHandlerType.Key),
                eventHandlerType.Value
            );
        }
    }

    /// <summary>
    /// Initializes the mime inspectors.
    /// </summary>
    public static void InitMimeInspectors(this IServiceCollection _)
    {
        MimeInspector.Init();
    }

    private static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
        where TOptions : class
    {
        serviceCollection
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(typeof(TOptions).Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return serviceCollection;
    }
}
