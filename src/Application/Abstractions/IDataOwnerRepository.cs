namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Interface for the data owner repository.
/// </summary>
public interface IDataOwnerRepository : ICrudRepository<DataOwner>
{
    /// <summary>
    /// Gets a data owner by its unique properties.
    /// </summary>
    /// <param name="name">The name of the data owner to get.</param>
    /// <param name="source">The source of the data owner to get.</param>
    /// <returns>A <see cref="Result"/> object representing the result of the operation.</returns>
    Result<DataOwner> GetByUniqueProperties(string name, string source);

    /// <summary>
    /// Gets all the email settings using the provided email template ID as default email template.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the data owner.</param>
    /// <returns>An <see cref="IEnumerable"/> with all the <see cref="EmailSettings"/>
    /// using <paramref name="emailTemplateId"/> as default email template.</returns>
    IEnumerable<EmailSettings> GetEmailSettingsByEmailTemplateId(Guid emailTemplateId);

    /// <summary>
    /// Delete or fall back on email settings containing the provided email template ID.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template belonging to the email settings.</param>
    /// <param name="fallBackEmailTemplate">The email template to fall back to.</param>
    /// <returns>A <see cref="Result"/> object representing the result of this operation.</returns>
    Result FallBackOrDeleteEmailSettings(
        Guid emailTemplateId,
        Result<EmailTemplate> fallBackEmailTemplate
    );
}
