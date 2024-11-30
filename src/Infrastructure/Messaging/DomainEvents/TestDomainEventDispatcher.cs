using MassTransit.Mediator;

namespace App.Server.Notification.Infrastructure.Messaging.DomainEvents;

/// <summary>
/// Test domain event dispatcher.
/// </summary>
public class TestDomainEventDispatcher(IScopedMediator mediator) : IDomainEventDispatcher
{
    /// <inheritdoc />
    public void DispatchAndClear(IEnumerable<Entity> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                mediator.Send(domainEvent).ConfigureAwait(false);
            }
        }
    }
}