using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Presentation.Controllers;

/// <summary>
/// Controller for managing the email queue.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class EmailQueueController(IMailingQueue mailingQueue) : ControllerBase
{
    /// <summary>
    /// Enqueues an email to be sent.
    /// </summary>
    /// <param name="emailInfoDto">The email information.</param>
    [HttpPost]
    public IActionResult Enqueue([FromBody] EmailInfoDto emailInfoDto) =>
        Ok(mailingQueue.EnqueueEmail(emailInfoDto));

    /// <summary>
    /// Schedules an email to be sent at a later time.
    /// </summary>
    /// <param name="scheduledEmailRequestDto">The scheduled email request information.</param>
    [HttpPost]
    public IActionResult Schedule([FromBody] ScheduledEmailRequestDto scheduledEmailRequestDto) =>
        Ok(
            mailingQueue.EnqueueScheduledEmail(
                scheduledEmailRequestDto.EmailInfoDto,
                scheduledEmailRequestDto.SendAt
            )
        );

    /// <summary>
    /// Adds a recurring email to the queue.
    /// </summary>
    /// <param name="recurringEmailRequestDto">The recurring email request information.</param>
    [HttpPost]
    public IActionResult AddRecurring([FromBody] RecurringEmailRequestDto recurringEmailRequestDto)
    {
        mailingQueue.AddRecurringEmail(
            recurringEmailRequestDto.JobId,
            recurringEmailRequestDto.EmailInfoDto,
            recurringEmailRequestDto.CronExpression
        );
        return Ok();
    }

    /// <summary>
    /// Dequeues a scheduled email.
    /// </summary>
    /// <param name="jobId">The job identifier to dequeue.</param>
    [HttpPost]
    public IActionResult DequeueScheduled([FromQuery] NonEmptyString jobId) =>
        Ok(mailingQueue.DequeueScheduledEmail(jobId));
}
