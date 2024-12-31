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
    public NonEmptyString EnqueueEmail(EmailInfo emailInfo) =>
        jobClient.Enqueue(() => emailSender.SendEmail(emailInfo));

    /// <inheritdoc />
    public NonEmptyString EnqueueScheduledEmail(EmailInfo emailInfo, DateTimeOffset sendAt) =>
        jobClient.Schedule(() => emailSender.SendEmail(emailInfo), sendAt);

    /// <inheritdoc />
    public void AddRecurringEmail(
        NonEmptyString jobId,
        EmailInfo emailInfo,
        NonEmptyString cronExpression
    ) =>
        recurringJobManager.AddOrUpdate(
            jobId,
            () => emailSender.SendEmail(emailInfo),
            cronExpression
        );

    /// <inheritdoc />
    public bool DequeueScheduledEmail(NonEmptyString jobId) => jobClient.Delete(jobId);
}
