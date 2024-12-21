namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Events;

/// <summary>
/// Event for when an email template is created.
/// </summary>
/// <param name="EmailTemplateId">The ID of the email template created.</param>
/// <param name="TemplateTypeId">The ID of the template type which this email template belongs to.</param>
/// <param name="OnlyEmailTemplateForType">Flag defining whether the new email template is the only one for its type.</param>
/// <param name="DataOwnerId">The ID of the data owner.</param>
public record EmailTemplateCreated(
    Guid EmailTemplateId,
    Guid TemplateTypeId,
    bool OnlyEmailTemplateForType,
    Guid? DataOwnerId
) : DomainEventBase;
