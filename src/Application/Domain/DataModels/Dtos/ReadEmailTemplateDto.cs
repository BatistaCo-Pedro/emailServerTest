namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for reading operation of email template.
/// </summary>
/// <param name="EmailBodyContents">The email body contents of the email template..</param>
/// <param name="IsCustom">Flag defining if the template is custom.</param>
public record ReadEmailTemplateDto(
    ImmutableHashSet<ReadEmailBodyContentDto> EmailBodyContents,
    bool IsCustom
) : IDto, IEventMessage;
