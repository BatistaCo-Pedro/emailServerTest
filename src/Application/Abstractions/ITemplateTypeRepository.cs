namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Interface for the template type repository.
/// </summary>
public interface ITemplateTypeRepository : ICrudRepository<TemplateType>
{
    /// <summary>
    /// Gets an email template by its ID.
    /// </summary>
    /// <param name="emailTemplateId">The email template to get.</param>
    /// <returns>The <see cref="emailTemplateId"/> matching the ID.</returns>
    Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="emailTemplate">The email template to delete.</param>
    /// <returns>A <see cref="Result"/> represnting the result of the operation.</returns>
    /// <remarks>
    /// This has to be done in the repository due to the delete behaviour of the EmailTemplate - TemplateType Relationship being defined as restrict.
    /// Severing the relationship is not enough to delete the email template.
    /// </remarks>
    Result DeleteEmailTemplate(EmailTemplate emailTemplate);
}
