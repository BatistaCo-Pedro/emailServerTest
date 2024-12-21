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
    /// <param name="emailInfo">The email information.</param>
    [HttpPost]
    public IActionResult Enqueue([FromBody] EmailInfo emailInfo) =>
        Ok(mailingQueue.EnqueueEmail(emailInfo));

    /// <summary>
    /// Schedules an email to be sent at a later time.
    /// </summary>
    /// <param name="scheduledEmailRequestDto">The scheduled email request information.</param>
    [HttpPost]
    public IActionResult Schedule([FromBody] ScheduledEmailRequestDto scheduledEmailRequestDto) =>
        Ok(
            mailingQueue.EnqueueScheduledEmail(
                scheduledEmailRequestDto.EmailInfo,
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
            recurringEmailRequestDto.EmailInfo,
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
