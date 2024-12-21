namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for creation operations of email templates.
/// </summary>
/// <param name="Name">A human-readable name to give to the new template.</param>
/// <param name="EmailBodyContentDto">The first content for this email template - is defined as default.</param>
/// <param name="TemplateTypeId">The ID of the type of the template.</param>
/// <param name="DataOwnerId">The ID of the data owner.</param>
[Serializable]
[method: JsonConstructor]
public record CreateEmailTemplateDto(
    NonEmptyString Name,
    CreateEmailBodyContentDto EmailBodyContentDto,
    Guid TemplateTypeId,
    Guid? DataOwnerId = null
) : IDto, IEventMessage;
