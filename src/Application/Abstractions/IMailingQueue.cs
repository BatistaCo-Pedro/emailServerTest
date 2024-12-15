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

public record EmailRequestDto(
    Guid TemplateTypeId,
    CultureCode CultureCode,
    NonEmptyString SenderName,
    NonEmptyString SenderEmail,
    NonEmptyString RecipientName,
    NonEmptyString RecipientEmail,
    ImmutableDictionary<string, object> MergeTagArguments,
    Guid? EmailTemplateId = null,
    NonEmptyString? CustomSubject = null
) : IDto, IEventMessage;

public record ScheduledEmailRequestDto(EmailRequestDto EmailRequestDto, DateTimeOffset SendTime)
    : IDto,
        IEventMessage;

public record RecurringEmailRequestDto(
    EmailRequestDto EmailRequestDto,
    string JobId,
    string CronExpression
) : IDto, IEventMessage;
