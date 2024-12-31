using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// Represents a scheduled email request.
/// </summary>
/// <param name="EmailInfoDto">The email information.</param>
/// <param name="SendAt">The <see cref="DateTimeOffset"/> to send the email at.</param>
public record ScheduledEmailRequestDto(
    [property: JsonPropertyName("emailInfo")] EmailInfoDto EmailInfoDto,
    [property: JsonPropertyName("sendAt")] DateTimeOffset SendAt
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledEmailRequestDto"/> record.
    /// </summary>
    /// <param name="emailInfoDto">The email information.</param>
    /// <param name="sendIn">The delay as <see cref="TimeSpan"/> to wait before sending the email.</param>
    public ScheduledEmailRequestDto(EmailInfoDto emailInfoDto, TimeSpan sendIn)
        : this(emailInfoDto, DateTime.UtcNow.Add(sendIn)) { }
}
