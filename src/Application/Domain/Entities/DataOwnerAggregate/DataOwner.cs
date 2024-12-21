#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate;

/// <summary>
/// The data owner is an outside entity that owns presets and custom templates.
/// This can be anything from a hotel to a restaurant, or even a person.
/// </summary>
public class DataOwner : AggregateRoot
{
    [Obsolete("Required by EF Core")]
    protected DataOwner() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataOwner"/> class.
    /// </summary>
    /// <param name="name">The name of the data owner.</param>
    /// <param name="source">The source of the data owner.</param>
    /// <param name="data">The data belonging to the data owner.</param>
    public DataOwner(
        NonEmptyString name,
        NonEmptyString source,
        ImmutableHashSet<CustomMergeTag> data
    )
    {
        Name = name;
        Source = source;
        Data = data;

        RaiseDomainEvent(new DataOwnerCreated(this));
    }

    /// <summary>
    /// Private set of custom merge tags for encapsulation. As <see cref="HashSet{T}"/> to prevent duplicates.
    /// </summary>
    private HashSet<CustomMergeTag> _data = [];

    /// <summary>
    /// Private set of email settings for encapsulation.
    /// </summary>
    private List<EmailSettings> _emailSettings = [];

    /// <summary>
    /// The name of the owner of the data.
    /// </summary>
    [StringLength(50)]
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// The source of the owner entity.
    /// </summary>
    [StringLength(50)]
    public NonEmptyString Source { get; private set; }

    /// <summary>
    /// The SMTP settings of the data owner.
    /// </summary>
    public virtual SmtpSettings? SmtpSettings { get; private set; }

    /// <summary>
    /// The data of the preset as <see cref="IReadOnlySet{T}"/> for encapsulation and to prevent duplicates.
    /// </summary>
    public IReadOnlySet<CustomMergeTag> Data
    {
        get => _data;
        private set => _data = value.ToHashSet();
    }

    /// <summary>
    /// The email settings of the data owner as <see cref="IReadOnlySet{T}"/> for encapsulation and to prevent duplicates.
    /// </summary>
    public virtual IReadOnlyCollection<EmailSettings> EmailSettings
    {
        get => _emailSettings;
        private set => _emailSettings = value.ToList();
    }

    /// <summary>
    /// Updates the custom merge tags of the data owner.
    /// </summary>
    /// <param name="newCustomMergeTags">The new <see cref="CustomMergeTag"/> list to set.</param>
    public void UpdateCustomMergeTags(ImmutableHashSet<CustomMergeTag> newCustomMergeTags) =>
        Data = newCustomMergeTags;

    /// <summary>
    /// Adds new email settings to the data owner.
    /// </summary>
    /// <param name="emailSettings">The <see cref="EmailSettings"/> to add.</param>
    public void AddEmailSettings(EmailSettings emailSettings) => _emailSettings.Add(emailSettings);

    /// <summary>
    /// Gets the email settings for a template type.
    /// </summary>
    /// <param name="templateTypeId">The template type ID to get the email settings for.</param>
    /// <returns>A <see cref="Result{TValue}"/> object representing the result of the operation.</returns>
    public Result<EmailSettings> GetEmailSettingsByTemplateTypeId(Guid templateTypeId) =>
        _emailSettings
            .FirstOrDefault(x => x.TemplateTypeId == templateTypeId)
            .ToResult(
                $"Data owner with ID {Id} does not contain email settings for template type with ID {templateTypeId}"
            );

    /// <summary>
    /// Gets the SMTP settings of the data owner.
    /// </summary>
    /// <returns>A <see cref="Result{TValue}"/> object representing the result of the operation.</returns>
    public Result<SmtpSettings> GetSmtpSettings() =>
        SmtpSettings.ToResult($"No SMTP settings found for data owner with ID {Id}.");

    /// <summary>
    /// Updates the SMTP settings of the data owner.
    /// </summary>
    /// <param name="smtpSettings">The smtp settings to update to.</param>
    public void UpdateSmtpSettings(SmtpSettings smtpSettings) => SmtpSettings = smtpSettings;
}
