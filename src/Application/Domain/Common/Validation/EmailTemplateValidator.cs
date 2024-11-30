namespace App.Server.Notification.Application.Domain.Common.Validation;

/// <summary>
/// Validation class for <see cref="EmailTemplate"/>.
/// </summary>
public class EmailTemplateValidator(EmailTemplate obj)
    : BaseValidator<EmailTemplate>(obj),
        IStaticValidator<EmailTemplate>
{
    /// <inheritdoc />
    public static IReadOnlyCollection<ValidationResult> Validate(EmailTemplate obj) =>
        new EmailTemplateValidator(obj).Validate();

    /// <inheritdoc />
    public static void ValidateAndThrow(EmailTemplate obj) =>
        new EmailTemplateValidator(obj).ValidateAndThrow();
}
