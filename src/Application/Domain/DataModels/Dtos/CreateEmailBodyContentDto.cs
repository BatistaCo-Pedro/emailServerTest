namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for creating email body contents.
/// </summary>
/// <param name="CultureCode">The culture of the content.</param>
/// <param name="Subject">The culture specific default subject of the content.</param>
/// <param name="Body">The body of the email.</param>
/// <param name="JsonStructure">The json structure of the email.</param>
/// <param name="IsDefault">Flag defining if the content is the default.</param>
[Serializable]
[method: JsonConstructor]
public record CreateEmailBodyContentDto(
    CultureCode CultureCode,
    NonEmptyString Subject,
    HtmlString Body,
    JsonDocument JsonStructure,
    bool IsDefault
) : IDto;
