using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing mailing operations.
/// </summary>
public interface IMailingService
{
    /// <summary>
    /// Gets a mail message based on the provided email info.
    /// </summary>
    /// <param name="dataOwner">The data owner.</param>
    /// <param name="emailInfo">The email info to create the mail message from.</param>
    /// <returns>A <see cref="Result{TValue}"/> of type <see cref="MailMessage"/> representing the result of the operation.</returns>
    Result<MailMessage> GetMailMessage(DataOwner dataOwner, EmailInfo emailInfo);
}

/// <inheritdoc />
public class MailingService(IUnitOfWork unitOfWork) : IMailingService
{
    /// <inheritdoc />
    public Result<MailMessage> GetMailMessage(DataOwner dataOwner, EmailInfo emailInfo)
    {
        var dataOwnerRepository = unitOfWork.GetRepository<ICrudRepository<DataOwner>>();
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        return dataOwner
            .GetEmailSettingsByTemplateTypeId(emailInfo.TemplateTypeId)
            .Match(emailSettings =>
                templateTypeRepository.GetEmailTemplate(
                    emailInfo.EmailTemplateId ?? emailSettings.DefaultEmailTemplateId
                )
            )
            .Match(emailTemplate => CreateMailMessage(dataOwner, emailInfo, emailTemplate));
    }

    private static Result<MailMessage> CreateMailMessage(
        DataOwner dataOwner,
        EmailInfo emailInfo,
        EmailTemplate emailTemplate
    )
    {
        return emailTemplate
            .GetContent(emailInfo.CultureCode)
            .Match(emailBodyContent =>
            {
                var plainBody = emailBodyContent.ToString();

                var emailBody = emailBodyContent.GetMergedBody(
                    emailInfo.MergeTagArguments,
                    dataOwner.Data.ToImmutableList(),
                    out var linkedResources
                );

                return Result<MailMessage>.Ok(
                    new EmailBuilder(
                        emailInfo.SenderMailAddress,
                        emailInfo.RecipientMailAddress,
                        emailInfo.CustomSubject ?? emailBodyContent.Subject
                    )
                        .AddPlain(plainBody)
                        .AddHtml(emailBody, builder => builder.AddLinkedResources(linkedResources))
                        .AddAttachments(emailInfo.Attachments)
                        .Build()
                );
            });
    }
}
