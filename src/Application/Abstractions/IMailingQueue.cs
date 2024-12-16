namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Interface for a mailing queue.
/// </summary>
public interface IMailingQueue
{
    public string EnqueueEmail(EmailRequestDto emailRequestDto);

    public string EnqueueScheduledEmail(ScheduledEmailRequestDto scheduledEmailRequestDto);

    //public bool DequeueScheduledEmail(string jobId);

    public void AddRecurringEmail(RecurringEmailRequestDto recurringEmailRequestDto);

    //public void RemoveRecurringEmail(string jobId);
}

public interface IMailingQueueViewer
{
    public bool PeekEmail(Guid emailTemplateId, out EmailRequestDto? emailRequestDto);
}

[Serializable]
[method: JsonConstructor]
public record EmailRequestDto(
    Guid TemplateTypeId,
    string CultureCode,
    string SenderName,
    string SenderEmail,
    string RecipientName,
    string RecipientEmail,
    ImmutableDictionary<string, object> MergeTagArguments,
    Guid? EmailTemplateId = null,
    string? CustomSubject = null
) : IDto, IEventMessage;

public record ScheduledEmailRequestDto(EmailRequestDto EmailRequestDto, TimeSpan SendIn)
    : IDto,
        IEventMessage;

public record RecurringEmailRequestDto(
    EmailRequestDto EmailRequestDto,
    string JobId,
    string CronExpression
) : IDto, IEventMessage;
