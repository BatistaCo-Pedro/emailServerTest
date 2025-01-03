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
    /// <param name="emailInfoDto">The email info to create the mail message from.</param>
    /// <returns>A <see cref="Result{TValue}"/> of type <see cref="MailMessage"/> representing the result of the operation.</returns>
    Result<MailMessage> GetMailMessage(DataOwner dataOwner, EmailInfoDto emailInfoDto);
}

/// <inheritdoc />
public class MailingService(IUnitOfWork unitOfWork) : IMailingService
{
    /// <inheritdoc />
    public Result<MailMessage> GetMailMessage(DataOwner dataOwner, EmailInfoDto emailInfoDto)
    {
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        return dataOwner
            .GetEmailSettingsByTemplateTypeId(emailInfoDto.TemplateTypeId)
            .Match(emailSettings =>
                templateTypeRepository.GetEmailTemplate(
                    emailInfoDto.EmailTemplateId ?? emailSettings.DefaultEmailTemplateId
                )
            )
            .Match(emailTemplate => CreateMailMessage(dataOwner, emailInfoDto, emailTemplate));
    }

    private static Result<MailMessage> CreateMailMessage(
        DataOwner dataOwner,
        EmailInfoDto emailInfoDto,
        EmailTemplate emailTemplate
    )
    {
        return emailTemplate
            .GetContent(emailInfoDto.CultureCode)
            .Match(emailBodyContent =>
            {
                var plainBody = emailBodyContent.ToString();

                var emailBody = emailBodyContent.GetMergedBody(
                    emailInfoDto.MergeTagArguments,
                    dataOwner.Data.ToImmutableHashSet(),
                    out var linkedResources
                );

                return Result<MailMessage>.Ok(
                    new EmailBuilder(
                        emailInfoDto.SenderMailAddress,
                        emailInfoDto.RecipientMailAddress,
                        emailInfoDto.CustomSubject ?? emailBodyContent.Subject
                    )
                        .AddPlain(plainBody)
                        .AddHtml(emailBody, builder => builder.AddLinkedResources(linkedResources))
                        .AddAttachments(emailInfoDto.Attachments)
                        .Build()
                );
            });
    }
}
