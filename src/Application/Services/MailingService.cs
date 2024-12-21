namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing mailing operations.
/// </summary>
public interface IMailingService
{
    /// <summary>
    /// Gets a mail message based on the provided email info.
    /// </summary>
    /// <param name="emailInfo">The email info to create the mail message from.</param>
    /// <returns>A <see cref="Result{TValue}"/> of type <see cref="MailMessage"/> representing the result of the operation.</returns>
    Result<MailMessage> GetMailMessage(EmailInfo emailInfo);
}

/// <inheritdoc />
public class MailingService(IUnitOfWork unitOfWork) : IMailingService
{
    /// <inheritdoc />
    public Result<MailMessage> GetMailMessage(EmailInfo emailInfo)
    {
        var dataOwnerRepository = unitOfWork.GetRepository<ICrudRepository<DataOwner>>();
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        return dataOwnerRepository
            .GetById(emailInfo.DataOwnerId)
            .Match(dataOwner =>
                dataOwner.GetEmailSettingsByTemplateTypeId(emailInfo.TemplateTypeId)
            )
            .Match(emailSettings =>
                templateTypeRepository.GetEmailTemplate(
                    emailInfo.EmailTemplateId ?? emailSettings.DefaultEmailTemplateId
                )
            )
            .Match(emailTemplate => CreateMailMessage(emailInfo, emailTemplate));
    }

    private static Result<MailMessage> CreateMailMessage(
        EmailInfo emailInfo,
        EmailTemplate emailTemplate
    )
    {
        return emailTemplate
            .GetContent(emailInfo.CultureCode)
            .Match(emailBodyContent =>
            {
                // TODO: var plainBody = emailBodyContent.ToString();

                // TODO: merging of merge tags with html body content
                // TODO: add support to have linked resources (also as merge tags)

                return Result<MailMessage>.Ok(
                    new EmailBuilder(
                        emailInfo.SenderMailAddress,
                        emailInfo.RecipientMailAddress,
                        emailInfo.CustomSubject ?? emailBodyContent.Subject
                    )
                        //.AddPlain(plainBody)
                        .AddHtml(emailBodyContent.Body)
                        .Build()
                );
            });
    }
}
