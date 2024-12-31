namespace App.Server.Notification.Application.Domain.DataModels.Emailing;

[Serializable]
[method: JsonConstructor]
public record AttachmentDto(
    [property: JsonPropertyName("value")] NonEmptyString Value,
    [property: JsonPropertyName("alt")] NonEmptyString Alt
)
{
    public ResourceDto ToResourceDto()
    {
        return Uri.TryCreate(Value, UriKind.Absolute, out var uri)
            ? new ResourceDto(uri, Alt)
            : new ResourceDto(Convert.FromBase64String(Value), Alt);
    }
}

[Serializable]
public record ResourceDto
{
    private static readonly ImmutableHashSet<string> UnsupportedMimeTypes =
    [
        MediaTypeNames.Image.Svg // unsafe
    ];

    [JsonConstructor]
    public ResourceDto(
        NonEmptyString value,
        NonEmptyString alt,
        NonEmptyString mimeType,
        short type
    )
    {
        Value = value;
        Alt = alt;
        MimeType = mimeType;
        Type = type;
    }

    public ResourceDto(Uri uri, NonEmptyString alt)
    {
        MimeType = MimeTypeHelper.GetMimeType(uri); 
        ThrowIfUnsupportedMimeType();

        Value = uri.AbsolutePath;
        Type = (short)ResourceType.Url;
        Alt = alt;
    }

    public ResourceDto(byte[] data, NonEmptyString alt)
    {
        MimeType = MimeTypeHelper.GetMimeType(data);
        ThrowIfUnsupportedMimeType();
        
        Value = Convert.ToBase64String(data);
        Type = (short)ResourceType.Base64;
        Alt = alt;
    }

    /// <summary>
    /// Either a Base64 encoded string or a URL to the resource.
    /// </summary>
    public NonEmptyString Value { get; init; }

    /// <summary>
    /// The alt value to use in case the resource can not be loaded.
    /// </summary>
    public NonEmptyString Alt { get; init; }

    /// <summary>
    /// The media type of the resource.
    /// </summary>
    public NonEmptyString MimeType { get; init; }

    /// <summary>
    /// The type of the resource.
    /// </summary>
    public short Type { get; init; }

    /// <summary>
    /// Creates a <see cref="Attachment"/> from the resource.
    /// </summary>
    /// <returns>A <see cref="Attachment"/> from the resource.</returns>
    public Attachment ToAttachment()
    {
        var attachment =
            Type == (short)ResourceType.Url
                ? new Attachment(Value, MimeType)
                : new Attachment(new MemoryStream(Convert.FromBase64String(Value)), null, MimeType);

        attachment.TransferEncoding = TransferEncoding.Base64;

        return attachment;
    }

    /// <summary>
    /// Creates a <see cref="LinkedResource"/> from the resource.
    /// </summary>
    /// <param name="contentId">The content ID to set.</param>
    /// <returns>A <see cref="LinkedResource"/> from the resource.</returns>
    public LinkedResource ToLinkedResource(NonEmptyString contentId)
    {
        if (!IsImage())
        {
            throw new ArgumentException("Linked resources must be images.");
        }

        var linkedResource =
            Type == (short)ResourceType.Url
                ? new LinkedResource(Value, MimeType)
                : new LinkedResource(new MemoryStream(Convert.FromBase64String(Value)), MimeType);

        linkedResource.ContentId = contentId;
        linkedResource.TransferEncoding = TransferEncoding.Base64;

        return linkedResource;
    }
      

    private bool IsImage() => MimeType.Value.StartsWith("image");

    private bool IsUnsupportedMimeType() =>
        UnsupportedMimeTypes.Contains(MimeType);

    private void ThrowIfUnsupportedMimeType()
    {
        if (IsUnsupportedMimeType())
        {
            throw new ArgumentException($"The MIME type {MimeType} is not supported.");
        }
    }
}

public enum ResourceType : short
{
    Url,
    Base64
}
