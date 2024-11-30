namespace App.Server.Notification.Application.Domain.Common.Validation;

/// <summary>
/// Validation class for <see cref="EmailBodyContent"/>.
/// </summary>
public class EmailBodyContentValidator
    : BaseValidator<EmailBodyContent>,
        IStaticValidator<EmailBodyContent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailBodyContentValidator"/> class.
    /// </summary>
    /// <param name="obj">The <see cref="EmailBodyContent"/> object to validate.</param>$
    // ReSharper disable once ConvertToPrimaryConstructor - Easier to read like this.
    public EmailBodyContentValidator(EmailBodyContent obj)
        : base(obj) { }

    /// <inheritdoc />
    public static IReadOnlyCollection<ValidationResult> Validate(
        EmailBodyContent emailBodyContent
    ) => new EmailBodyContentValidator(emailBodyContent).Validate();

    /// <inheritdoc />
    public static void ValidateAndThrow(EmailBodyContent emailBodyContent) =>
        new EmailBodyContentValidator(emailBodyContent).ValidateAndThrow();
}
