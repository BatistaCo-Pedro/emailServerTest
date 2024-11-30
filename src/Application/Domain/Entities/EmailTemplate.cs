// ReSharper disable UnusedMember.Local
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace App.Server.Notification.Application.Domain.Entities;

/// <summary>
/// Email template entity.
/// </summary>
public class EmailTemplate : AuditableEntity
{
    [Obsolete("Required by DI and EF Core")]
    protected EmailTemplate() { }

    /// <summary>
    /// Ctor. Creates an instance of <see cref="EmailTemplate"/>.
    /// </summary>
    /// <param name="name">A human-readable name for the template.</param>
    /// <param name="isCustom">A flag defining whether the template is custom.</param>
    /// <param name="templateTypeId">The ID of the type of the template.</param>
    /// <remarks>
    /// Email templates always have at least one email content body.
    /// </remarks>
    public EmailTemplate(
        NonEmptyString name,
        bool isCustom,
        Guid templateTypeId
    )
    {
        Name = name;
        TemplateTypeId = templateTypeId;
        IsCustom = isCustom;

        EmailTemplateValidator.ValidateAndThrow(this);
    }

    /// <summary>
    /// Private list of email presets for encapsulation.
    /// </summary>
    private readonly List<EmailPreset> _emailPresets = [];

    /// <summary>
    /// Private list of email body contents for encapsulation.
    /// </summary>
    private readonly List<EmailBodyContent> _emailBodyContents = [];

    /// <summary>
    /// Human-readable name of the template.
    /// </summary>
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// Flag defining if the template is custom.
    /// </summary>
    public bool IsCustom { get; private set; }

    /// <summary>
    /// The email presets of this template as <see cref="IReadOnlyCollection{T}"/> for encapsulation.
    /// </summary>
    public virtual IReadOnlyCollection<EmailPreset> EmailPresets
    {
        get => _emailPresets;
        private init => _emailPresets = value.ToList();
    }

    /// <summary>
    /// The email body contents of this template as <see cref="IReadOnlyCollection{T}"/> for encapsulation.
    /// </summary>
    public virtual IReadOnlyCollection<EmailBodyContent> EmailBodyContents
    {
        get => _emailBodyContents;
        private init => _emailBodyContents = value.ToList();
    }

    /// <summary>
    /// The type of the template.
    /// </summary>
    public virtual TemplateType TemplateType { get; private set; }

    /// <summary>
    /// The ID of the template type.
    /// </summary>
    [ForeignKey(nameof(TemplateType))]
    public Guid TemplateTypeId { get; private set; }

    /// <summary>
    /// The owning entity of the template - only populated if the template is custom and has an owner.
    /// </summary>
    public virtual DataOwner? DataOwner { get; private set; }

    /// <summary>
    /// The ID of the owner of this custom template.
    /// </summary>
    [ForeignKey(nameof(DataOwner))]
    public Guid? DataOwnerId { get; private set; }

    /// <summary>
    /// Adds a preset to the email template.
    /// </summary>
    /// <param name="emailPreset">The email preset entity to add.</param>
    public void AddPreset(EmailPreset emailPreset)
    {
        // TODO: Validations
        _emailPresets.Add(emailPreset);
    }

    /// <summary>
    /// Gets a preset belonging to the email template.
    /// </summary>
    /// <param name="emailPresetId">The id of the email preset to get.</param>
    /// <returns>The <see cref="EmailPreset"/> belonging the email template if present.</returns>
    /// <exception cref="ArgumentException">The email template doesn't contain the preset with the specified id.</exception>
    public EmailPreset GetEmailPreset(Guid emailPresetId) =>
        _emailPresets.FirstOrDefault(x => x.Id == emailPresetId)
        ?? throw new ArgumentException(
            $"Email preset {Id} doesn't contain email preset with ID {emailPresetId}",
            nameof(emailPresetId)
        );

    /// <summary>
    /// Removes a preset from the email template.
    /// </summary>
    /// <param name="emailPreset">The email preset entity to remove.</param>
    public void RemovePreset(EmailPreset emailPreset)
    {
        // TODO: check if email preset is being used somewhere
        _emailPresets.Remove(emailPreset);
    }

    /// <summary>
    /// Assigns a data owner to the email template.
    /// </summary>
    /// <param name="dataOwnerId">The ID of the data owner to assign.</param>
    /// TODO: Create a ctor. for custom templates instead of adding a data owner.
    public void AssignDataOwner(Guid dataOwnerId)
    {
        DataOwnerId = dataOwnerId;
        IsCustom = true;
    }

    /// <summary>
    /// Gets the email body for the given culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code to get the email body for.</param>
    /// <returns>
    /// A <see cref="EmailBodyContent"/> belonging to the email template matching the provided culture code.
    /// </returns>
    /// <exception cref="ArgumentException">Body couldn't be found for provided culture code.</exception>
    public Result<EmailBodyContent> GetContent(CultureCode cultureCode) =>
        Result<EmailBodyContent>.Try(
            () =>
                _emailBodyContents.FirstOrDefault(x => x.CultureCode == cultureCode)
                ?? throw new ArgumentException(
                    $"Couldn't find a body for the culture code {cultureCode}",
                    nameof(cultureCode)
                )
        );

    /// <summary>
    /// Updates the content of the email template.
    /// This method updates contents if they exist, otherwise it adds them.
    /// </summary>
    /// <param name="emailBodyContentDto">The dto of the content to update or add.</param>
    /// <returns>A <see cref="Result"/> object representing the success of this operation.</returns>
    public Result UpdateContent(EmailBodyContentDto emailBodyContentDto)
    {
        var emailBodyContent = emailBodyContentDto.ToEntity(
            MergeTagHelper.GetMergeTags(
                emailBodyContentDto.JsonStructure,
                TemplateType.AcceptedMergeTags.ToImmutableHashSet()
            )
        );

        var dbEmailBodyContent = _emailBodyContents.FirstOrDefault(x =>
            x.CultureCode == emailBodyContent.CultureCode
        );

        return dbEmailBodyContent is null
            ? AddContent(emailBodyContent)
            : UpdateContent(emailBodyContent, dbEmailBodyContent);
    }

    private Result AddContent(EmailBodyContent emailBodyContent) =>
        Result.Try(() =>
        {
            UpdateDefaultContent(emailBodyContent);
            _emailBodyContents.Add(emailBodyContent);
        });

    private Result UpdateContent(
        EmailBodyContent emailBodyContent,
        EmailBodyContent dbEmailBodyContent
    ) =>
        Result.Try(() =>
        {
            UpdateDefaultContent(emailBodyContent);
            dbEmailBodyContent.SetDefault(emailBodyContent.IsDefault);

            dbEmailBodyContent
                .UpdateBody(
                    emailBodyContent.Body,
                    emailBodyContent.JsonStructure,
                    emailBodyContent.MergeTags
                )
                .ThrowIfFailed();
        });

    private void UpdateDefaultContent(EmailBodyContent emailBodyContent)
    {
        if (emailBodyContent.IsDefault)
        {
            _emailBodyContents.Single(x => x.IsDefault).SetDefault(false);
        }
    }
}
