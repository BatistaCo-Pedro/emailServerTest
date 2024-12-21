namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for email settings.
/// </summary>
/// <param name="DataOwnerId">The ID of the data owner.</param>
/// <param name="TemplateTypeId">The ID of the template type.</param>
/// <param name="DefaultEmailTemplateId">The ID of the new default email template.</param>
public record EmailSettingsDto(Guid DataOwnerId, Guid TemplateTypeId, Guid DefaultEmailTemplateId);
