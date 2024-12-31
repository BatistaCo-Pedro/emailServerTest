using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Application.Services;

/// <summary>
/// The email sender.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="emailInfo">The <see cref="EmailInfo"/> object containing all the required email info.</param>
    /// <returns>A <see cref="Result"/> object representing the result of the operation.</returns>
    Result SendEmail(EmailInfo emailInfo);
}

/// <inheritdoc />
public class EmailSender(
    IUnitOfWork unitOfWork,
    IMailingService mailingService,
    IOptions<SmtpSettings> defaultSmtpSettings
) : IEmailSender
{
    /// <inheritdoc />
    public Result SendEmail(EmailInfo emailInfo)
    {
        var dataOwnerRepo = unitOfWork.GetRepository<IDataOwnerRepository>();
        var dataOwnerResult = dataOwnerRepo.GetById(emailInfo.DataOwnerId);

        if (!dataOwnerResult.IsSuccess)
        {
            return dataOwnerResult.Errors;
        }

        var smtpClient = dataOwnerResult
            .Value.GetSmtpSettings()
            .Match(CreateSmtpClient, _ => CreateSmtpClient(defaultSmtpSettings.Value));

        return mailingService
            .GetMailMessage(dataOwnerResult.Value, emailInfo)
            .Match(mailMessage => Result.Try(() => smtpClient.Send(mailMessage)));
    }

    private static SmtpClient CreateSmtpClient(SmtpSettings smtpSettings) =>
        new(smtpSettings.Host, smtpSettings.Port)
        {
            DeliveryMethod = Enum.Parse<SmtpDeliveryMethod>(smtpSettings.DeliveryMethod),
            EnableSsl = smtpSettings.EnableSsl,
            Credentials = smtpSettings.Password is not null
                ? new NetworkCredential(smtpSettings.Address, smtpSettings.Password)
                : null,
        };
}
