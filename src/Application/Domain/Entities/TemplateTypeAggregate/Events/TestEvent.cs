namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Events;

/// <summary>
/// Test event class.
/// </summary>
public record TestEvent(long Id, string Text) : IDomainEvent;