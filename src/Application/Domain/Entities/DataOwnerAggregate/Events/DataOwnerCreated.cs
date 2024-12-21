namespace App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate.Events;

/// <summary>
/// Event for when a data owner is created.
/// </summary>
/// <param name="DataOwner">The created data owner.</param>
public record DataOwnerCreated(DataOwner DataOwner) : DomainEventBase;
