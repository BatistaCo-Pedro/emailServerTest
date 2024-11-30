using App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Events;

namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Handlers;

/// <inheritdoc /> 
public class TestEventHandler : IEventHandler<TestEvent>
{
    /// <inheritdoc /> 
    public Task Handle(TestEvent eventMessage)
    {
        Log.Warning("Received event: {Event}", eventMessage);
        return Task.CompletedTask;
    }
}