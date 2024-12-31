// ReSharper disable UnusedAutoPropertyAccessor.Local

using App.Server.Notification.Application.Domain.DataModels.Emailing;
using HandlebarsDotNet;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate;

/// <summary>
/// Culture specific email body entity.
/// </summary>
public class EmailBodyContent : AuditableEntity
{
    [Obsolete("Required by DI and EF Core")]
    protected EmailBodyContent() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailBodyContent"/> class.
    /// </summary>
    /// <param name="cultureCode">The culture code of the culture.</param>
    /// <param name="subject">The culture specific subject for the content.</param>
    /// <param name="body">The email body of the content.</param>
    /// <param name="jsonStructure">The JSON structure of the email.</param>
    /// <param name="acceptedMergeTags">The accepted merge tags from the type.</param>
    /// <param name="isDefault">Defines if the content is the default content.</param>
    /// <remarks>
    /// <paramref name="jsonStructure"/> and <paramref name="body"/> get compressed and decompressed when being inserted into the database.
    /// <br/>
    /// See <see cref="StringCompressor"/> and database model configuration for more information.
    /// </remarks>
    public EmailBodyContent(
        CultureCode cultureCode,
        NonEmptyString subject,
        HtmlString body,
        JsonDocument jsonStructure,
        ImmutableHashSet<MergeTag> acceptedMergeTags,
        bool isDefault
    )
    {
        CultureCode = cultureCode;
        Subject = subject;
        Body = body;
        JsonStructure = jsonStructure;
        IsDefault = isDefault;
        MergeTags = MergeTagHelper.GetMergeTags(jsonStructure, acceptedMergeTags).Value; // throws on failure
    }

    private HashSet<MergeTag> _mergeTags = [];

    /// <summary>
    /// Culture specific default subject for a content. May contain merge tags.
    /// </summary>
    public NonEmptyString Subject { get; private set; }

    /// <summary>
    /// Defines if this email body is the default to use in case no email body gets found for a culture.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// The email template this culture specific email body belongs to.
    /// </summary>
    public virtual EmailTemplate EmailTemplate { get; private set; }

    /// <summary>
    /// The ID of the email template this culture specific email body belongs to.
    /// </summary>
    [ForeignKey(nameof(EmailTemplate))]
    public Guid EmailTemplateId { get; private set; }

    /// <summary>
    /// The required data for the templates <see cref="IReadOnlySet{T}"/> for encapsulation and to prevent duplicates.
    /// </summary>
    [NotEmpty]
    public IReadOnlySet<MergeTag> MergeTags
    {
        get => _mergeTags;
        private set => _mergeTags = value.ToHashSet();
    }

    /// <summary>
    /// The email body of this content.
    /// </summary>
    public HtmlString Body { get; private set; }

    /// <summary>
    /// The JSON structure of this content.
    /// </summary>
    public JsonDocument JsonStructure { get; private set; }

    /// <summary>
    /// The valid culture code of the email body.
    /// </summary>
    /// <remarks>
    /// The culture code needs to be unique per email template. Key { CultureCode, EmailTemplateId }
    /// </remarks>
    public CultureCode CultureCode { get; private set; }

    /// <summary>
    /// Updates the content body.
    /// </summary>
    /// <param name="body">The new email body to set.</param>
    /// <param name="jsonStructure">The new JSON structure to set.</param>
    /// <returns>A <see cref="Result"/> representing the operation.</returns>
    public Result UpdateBody(HtmlString body, JsonDocument jsonStructure)
    {
        // TODO: Validations

        Body = body;
        JsonStructure = jsonStructure;

        var newMergeTagsResult = MergeTagHelper.GetMergeTags(
            jsonStructure,
            EmailTemplate.TemplateType.AcceptedMergeTags
        );

        return newMergeTagsResult.Match(newMergeTags =>
        {
            MergeTags = newMergeTags;
            return Result.Ok();
        });
    }

    /// <summary>
    /// Sets the default value for the email body.
    /// </summary>
    /// <param name="isDefault">Defines if the email body is the default email body.</param>
    public void SetDefault(bool isDefault)
    {
        Log.Information(
            "{EmailBodyId} with the culture {CultureCode} is the new default email body content for {EmailTemplateId}",
            Id,
            CultureCode,
            EmailTemplateId
        );
        IsDefault = isDefault;
    }

    /// <summary>
    /// Merges the email body content with the merge tag parameters and custom data.
    /// </summary>
    /// <param name="mergeTagParameters"></param>
    /// <param name="customData"></param>
    /// <param name="linkedResources"></param>
    /// <returns></returns>
    public HtmlString GetMergedBody(
        ImmutableDictionary<string, object> mergeTagParameters,
        ImmutableList<CustomMergeTag> customData,
        out List<LinkedResource> linkedResources
    )
    {
        linkedResources = [];

        var lookupByImage = MergeTagHelper.GetLookupByImageType(
            MergeTags.ToImmutableHashSet(),
            mergeTagParameters,
            customData
        );

        foreach (var (name, value) in lookupByImage[true].ToHashSet())
        {
            var image = new ResourceDto(Convert.FromBase64String((string)value), name);
            linkedResources.Add(image.ToLinkedResource(name));
        }

        var template = Handlebars.Compile(Body);
        var data = lookupByImage[false].ToDictionary();
        return template(data);
    }

    /// <summary>
    /// Returns the plain text representation of the email body content.
    /// </summary>
    /// <returns>The plain text string.</returns>
    public override string ToString() => Body.StripHtml();
}
