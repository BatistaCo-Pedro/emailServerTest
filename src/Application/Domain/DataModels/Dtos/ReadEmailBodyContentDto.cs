namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for reading email body contents.
/// </summary>
/// <param name="CultureCode">The culture code of the content.</param>
/// <param name="Subject">The culture specific default subject of the content.</param>
/// <param name="JsonStructure">The json structure of the email.</param>
/// <param name="IsDefault">Flag defining if the content is the default.</param>
[Serializable]
[method: JsonConstructor]
public record ReadEmailBodyContentDto(
    CultureCode CultureCode,
    NonEmptyString Subject,
    JsonDocument JsonStructure,
    bool IsDefault
) : IDto;
