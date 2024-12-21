namespace App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate;

/// <summary>
/// Represents the email settings for a data owner.
/// </summary>
public class EmailSettings : AuditableEntity
{
    [Obsolete("Required by EF Core")]
    protected EmailSettings() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSettings"/> class.
    /// </summary>
    /// <param name="dataOwnerId">The ID of the data owner these settings belong to.</param>
    /// <param name="templateTypeId">The ID of the template type these settings apply to.</param>
    /// <param name="defaultEmailTemplateId">The ID of the default email template for this data owner for this template type.</param>
    public EmailSettings(Guid dataOwnerId, Guid templateTypeId, Guid defaultEmailTemplateId)
    {
        DataOwnerId = dataOwnerId;
        TemplateTypeId = templateTypeId;
        DefaultEmailTemplateId = defaultEmailTemplateId;
    }

    public Guid DataOwnerId { get; private set; }

    /// <summary>
    /// The ID of the template type these settings apply to.
    /// </summary>
    public Guid TemplateTypeId { get; private set; }

    /// <summary>
    /// The ID of the default email template for this data owner for this template type.
    /// </summary>
    public Guid DefaultEmailTemplateId { get; private set; }

    /// <summary>
    /// Replace the default email template with a new one.
    /// </summary>
    /// <param name="newDefaultEmailTemplateId">The ID of the new default email template.</param>
    public void ReplaceDefaultEmailTemplate(Guid newDefaultEmailTemplateId)
    {
        DefaultEmailTemplateId = newDefaultEmailTemplateId;
    }
}
