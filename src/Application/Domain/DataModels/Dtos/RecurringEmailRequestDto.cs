using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// Represents a recurring email request.
/// </summary>
/// <param name="JobId">The identifier of the recurring job.</param>
/// <param name="EmailInfoDto">The email information.</param>
/// <param name="CronExpression">The cron expression representing when to send the email.</param>
public record RecurringEmailRequestDto(
    [property: JsonPropertyName("jobId")] NonEmptyString JobId,
    [property: JsonPropertyName("emailInfo")] EmailInfoDto EmailInfoDto,
    [property: JsonPropertyName("cronExpression")] NonEmptyString CronExpression
);
