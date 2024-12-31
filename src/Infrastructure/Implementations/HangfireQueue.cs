using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Infrastructure.Implementations;

/// <summary>
/// Mailing queue using Hangfire.
/// </summary>
/// <param name="jobClient">Hangfire job client.</param>
/// <param name="recurringJobManager">Hangfire recurring job manager.</param>
/// <param name="emailSender">The email sender to send emails.</param>
internal class HangfireQueue(
    IBackgroundJobClientV2 jobClient,
    IRecurringJobManagerV2 recurringJobManager,
    IEmailSender emailSender
) : IMailingQueue
{
    /// <inheritdoc />
    public NonEmptyString EnqueueEmail(EmailInfoDto emailInfoDto) =>
        jobClient.Enqueue(() => emailSender.SendEmail(emailInfoDto));

    /// <inheritdoc />
    public NonEmptyString EnqueueScheduledEmail(EmailInfoDto emailInfoDto, DateTimeOffset sendAt) =>
        jobClient.Schedule(() => emailSender.SendEmail(emailInfoDto), sendAt);

    /// <inheritdoc />
    public void AddRecurringEmail(
        NonEmptyString jobId,
        EmailInfoDto emailInfoDto,
        NonEmptyString cronExpression
    ) =>
        recurringJobManager.AddOrUpdate(
            jobId,
            () => emailSender.SendEmail(emailInfoDto),
            cronExpression
        );

    /// <inheritdoc />
    public bool DequeueScheduledEmail(NonEmptyString jobId) => jobClient.Delete(jobId);
}
