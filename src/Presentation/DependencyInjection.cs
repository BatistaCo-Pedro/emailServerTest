namespace App.Server.Notification.Presentation;

/// <summary>
/// Extension class containing dependency injection methods for the presentation layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds services belonging to the presentation layer.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHealthChecks();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Presentation", Version = "v1" });
        });

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        // Add event handlers

        services.AddLocalization();

        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    /// <summary>
    /// Adds serilog logging to the application.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        Log.Logger = loggerConfig;
        builder.Host.UseSerilog(loggerConfig);

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(loggerConfig);

        return builder;
    }
}
