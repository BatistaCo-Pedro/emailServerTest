namespace App.Server.Notification.Infrastructure.Messaging.DomainEvents;

/// <summary>
/// Default implementation of the domain event dispatcher.
/// </summary>
public class DefaultDomainEventDispatcher : IDomainEventDispatcher
{
    /// <inheritdoc />
    public void DispatchAndClear(IEnumerable<Entity> entitiesWithEvents)
    {
        Log.Warning("No domain event dispatcher configured.");
    }
}
