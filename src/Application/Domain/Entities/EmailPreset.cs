// ReSharper disable UnusedMember.Local
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace App.Server.Notification.Application.Domain.Entities;

/// <summary>
/// Email presets contain pre-defined data based on their data owners and email templates.
/// This is for example the name of the sender or the address, as this data doesn't change for every notification.
/// </summary>
public class EmailPreset : AuditableEntity
{
    [Obsolete("Required by EF Core")]
    protected EmailPreset() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailPreset"/> class.
    /// </summary>
    /// <param name="customMergeTags">The data for the preset - must match the required data by the email template.</param>
    /// <param name="dataOwnerId">The ID of the owner of this preset.</param>
    public EmailPreset(ImmutableHashSet<CustomMergeTag> customMergeTags, Guid dataOwnerId)
    {
        CustomMergeTags = customMergeTags;
        DataOwnerId = dataOwnerId;
    }

    /// <summary>
    /// Private set of custom merge tags for encapsulation. As <see cref="HashSet{T}"/> to prevent duplicates.
    /// </summary>
    private readonly HashSet<CustomMergeTag> _customMergeTags = []; // set default provided values

    /// <summary>
    /// The data of the preset as <see cref="IReadOnlySet{T}"/> for encapsulation and to prevent duplicates.
    /// </summary>
    public IReadOnlySet<CustomMergeTag> CustomMergeTags
    {
        get => _customMergeTags;
        private init => _customMergeTags = value.ToHashSet();
    }

    /// <summary>
    /// The owner of this preset.
    /// </summary>
    public virtual DataOwner DataOwner { get; private set; }

    /// <summary>
    /// The ID of the owner of this preset.
    /// </summary>
    [ForeignKey(nameof(DataOwner))]
    public Guid DataOwnerId { get; private set; }

    /// <summary>
    /// The email template this preset belongs to.
    /// </summary>
    public virtual EmailTemplate EmailTemplate { get; private set; }

    /// <summary>
    /// The ID of the email template this preset belongs to.
    /// </summary>
    [ForeignKey(nameof(EmailTemplate))]
    public Guid EmailTemplateId { get; private set; }

    /// <summary>
    /// Adds custom merge tags to the preset.
    /// </summary>
    /// <param name="newCustomMergeTags">The custom merge tags to add.</param>
    /// <returns>A <see cref="Result"/> representing the success of the operation.</returns>
    public Result AddCustomMergeTags(ImmutableHashSet<CustomMergeTag> newCustomMergeTags)
    {
        return Result.Try(() =>
        {
            _customMergeTags.UnionWith(newCustomMergeTags);
        });
    }
}
