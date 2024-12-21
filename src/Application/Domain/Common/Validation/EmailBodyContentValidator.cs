namespace App.Server.Notification.Application.Domain.Common.Validation;

/// <summary>
/// Validation class for <see cref="EmailBodyContent"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailBodyContentValidator"/> class.
/// </remarks>
/// <param name="obj">The <see cref="EmailBodyContent"/> object to validate.</param>$
public class EmailBodyContentValidator(EmailBodyContent obj)
    : BaseValidator<EmailBodyContent>(obj),
        IStaticValidator<EmailBodyContent>
{
    /// <inheritdoc />
    public static IReadOnlyCollection<ValidationResult> Validate(
        EmailBodyContent emailBodyContent
    ) => new EmailBodyContentValidator(emailBodyContent).Validate();

    /// <inheritdoc />
    public static void ValidateAndThrow(EmailBodyContent emailBodyContent) =>
        new EmailBodyContentValidator(emailBodyContent).ValidateAndThrow();
}
