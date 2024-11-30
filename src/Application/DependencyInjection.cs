using App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Events;
using App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Handlers;

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
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IDataOwnerService, DataOwnerService>();
        services.AddScoped<ITemplateTypeService, TemplateTypeService>();

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventHandler<TestEvent>, TestEventHandler>();
        
        return serviceCollection;
    }
}
