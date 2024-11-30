namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for reading operation of email template.
/// </summary>
/// <param name="EmailJsonStructures">The JSON structures of the emails with their culture specific content.</param>
/// <param name="IsCustom">Flag defining if the template is custom.</param>
public record ReadEmailTemplateDto(ImmutableList<JsonDocument> EmailJsonStructures, bool IsCustom)
    : IDto,
        IEventMessage;
