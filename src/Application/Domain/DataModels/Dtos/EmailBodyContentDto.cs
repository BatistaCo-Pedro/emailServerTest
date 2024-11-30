namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for email body content.
/// </summary>
/// <param name="CultureCode">The culture of the content.</param>
/// <param name="CultureCode">The culture specific default subject of the content.</param>
/// <param name="Body">The body of the email.</param>
/// <param name="JsonStructure">The body of the e.</param>
/// <param name="IsDefault">Flag defining if the content is the default.</param>
[Serializable]
[method: JsonConstructor]
public record EmailBodyContentDto(
    CultureCode CultureCode,
    NonEmptyString Subject,
    HtmlString Body,
    JsonDocument JsonStructure,
    bool IsDefault
) : IDto;
