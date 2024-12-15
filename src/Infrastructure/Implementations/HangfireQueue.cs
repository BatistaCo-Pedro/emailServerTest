namespace App.Server.Notification.Infrastructure.Implementations;

/// <summary>
/// Mailing queue using Hangfire.
/// </summary>
/// <param name="jobClient">Hangfire job client dependency.</param>
/// <param name="recurringJobManager">Hangfire recurring job manager dependency.</param>
/// <param name="unitOfWork">Unit of work dependency.</param>
/// TODO: Implement actual methods and remove comments.
internal class HangfireQueue(
    IBackgroundJobClientV2 jobClient,
    IRecurringJobManager recurringJobManager,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender
) : IMailingQueue
{
    public string EnqueueEmail(EmailRequestDto emailRequestDto) =>
        jobClient.Enqueue(() => emailSender.SendEmail(emailRequestDto));

    public string EnqueueScheduledEmail(ScheduledEmailRequestDto scheduledEmailRequestDto) =>
        jobClient.Schedule(
            () => emailSender.SendEmail(scheduledEmailRequestDto.EmailRequestDto),
            scheduledEmailRequestDto.SendTime
        );

    public void AddRecurringEmail(RecurringEmailRequestDto recurringEmailRequestDto) =>
        recurringJobManager.AddOrUpdate(
            recurringEmailRequestDto.JobId,
            () => emailSender.SendEmail(recurringEmailRequestDto.EmailRequestDto),
            recurringEmailRequestDto.CronExpression
        );
}

public interface IEmailSender
{
    void SendEmail(EmailRequestDto emailRequestDto);
}

public class EmailSender(IUnitOfWork unitOfWork) : IEmailSender
{
    public void SendEmail(EmailRequestDto emailRequestDto)
    {
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        // if only type is provided, get the default template
        // var emailTemplate = templateType.DefaultEmailTemplate

        // else
        // templateType.GetTemplate(emailRequestDto.TemplateType)


        var emailTemplateResult = templateTypeRepository.GetEmailTemplate(
            emailRequestDto.EmailTemplateId!.Value
        );

        if (!emailTemplateResult.IsSuccess)
        {
            return;
        }

        var emailTemplate = emailTemplateResult.Value;

        var emailBodyContentResult = emailTemplate.GetContent(emailRequestDto.CultureCode);

        if (!emailBodyContentResult.IsSuccess)
        {
            return;
        }

        var emailBodyContent = emailBodyContentResult.Value;

        // Build email
        // var mailMessage = MailMessageBuilder.Build(emailInfoDto, emailBodyContent);

        // Send email
        // mail.Send();
    }
}

public class MailingQueueViewer(IBackgroundJobClientV2 jobClient) : IMailingQueueViewer
{
    public bool PeekEmail(Guid emailTemplateId, out EmailRequestDto? emailRequestDto)
    {
        var jobs = jobClient.Storage.GetMonitoringApi().EnqueuedJobs("default", 0, 1);

        emailRequestDto = (EmailRequestDto?)
            jobs.Select(x =>
                    x.Value.Job.Args.FirstOrDefault(y =>
                        y is EmailRequestDto emailInfoDto
                        && emailInfoDto.EmailTemplateId == emailTemplateId
                    )
                )
                .FirstOrDefault();

        return emailRequestDto is not null;
    }
}
