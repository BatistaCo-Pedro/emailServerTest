namespace App.Server.Notification.Infrastructure.Messaging.DomainEvents;

/// <summary>
/// Interface for domain event dispatchers.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches and clears all domain events from the given entities.
    /// </summary>
    /// <param name="entitiesWithEvents">The entities to dispatch and clear events for.</param>
    public void DispatchAndClear(IEnumerable<Entity> entitiesWithEvents);
}
