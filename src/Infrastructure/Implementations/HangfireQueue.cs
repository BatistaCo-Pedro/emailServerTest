namespace App.Server.Notification.Infrastructure.Implementations;

/// <summary>
/// Mailing queue using Hangfire.
/// </summary>
/// <param name="jobClient">Hangfire job client dependency.</param>
/// <param name="recurringJobManager">Hangfire recurring job manager dependency.</param>
/// <param name="unitOfWork">Unit of work dependency.</param>
/// TODO: Implement actual methods and remove comments.
internal class HangfireQueue(
    IBackgroundJobClient jobClient,
    IRecurringJobManager recurringJobManager,
    IUnitOfWork unitOfWork
) : IMailingQueue
{
    // Implementation of IMailingQueue
}
