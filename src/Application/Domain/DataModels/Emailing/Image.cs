namespace App.Server.Notification.Application.Domain.DataModels.Emailing;

/// <summary>
///
/// </summary>
[Serializable]
public record Image : Resource
{
    /// <summary>
    /// The supported image extensions.
    /// </summary>
    private static readonly ImmutableHashSet<string> SupportedExtensions = new[]
    {
        "jpg",
        "jpeg",
        "png",
        "gif",
        "bmp",
        "tif",
        "tiff",
        "svg", // should be validated
        "webp"
    }.ToImmutableHashSet(StringComparer.InvariantCultureIgnoreCase);
    
    /// <summary>
    /// Suffix for the content ID of the image - used for <see cref="LinkedResource"/>.
    /// </summary>
    public const string IdSuffix = "cid:";

    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="alt">The alternative value.</param>
    /// <param name="mediaType">The media type.</param>
    /// <remarks>
    /// This constructor is used for serialization.
    /// </remarks>
    [JsonConstructor]
    public Image(NonEmptyString value, NonEmptyString alt, NonEmptyString mediaType)
        : base(value, alt, mediaType) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    /// <param name="value">Is either an url to a supported image type on a content server
    /// <br/>
    /// -- or --
    /// <br/>
    /// a base64 encoded string of an image.
    /// </param>
    /// <param name="alt">The alternative value of the image to be shown in case it can not be loaded.</param>
    public Image(NonEmptyString value, NonEmptyString alt)
        : base(value, alt)
    {
        // Validations
        if (!SupportedExtensions.Contains(MediaType))
        {
            throw new ArgumentException($"The media type '{MediaType}' is not supported.");
        }
    }

    /// <summary>
    /// Creates a <see cref="LinkedResource"/> from the image.
    /// </summary>
    /// <param name="contentId">The content ID to set.</param>
    /// <returns>A <see cref="LinkedResource"/> from the image.</returns>
    public LinkedResource ToLinkedResource(NonEmptyString contentId)
    {
        var linkedResource =
            Type == (short)ResourceType.Url
                ? new LinkedResource(Value, MediaType)
                : new LinkedResource(new MemoryStream(Convert.FromBase64String(Value)), MediaType);

        linkedResource.ContentId = contentId;
        linkedResource.TransferEncoding = TransferEncoding.Base64;

        return linkedResource;
    }
}
