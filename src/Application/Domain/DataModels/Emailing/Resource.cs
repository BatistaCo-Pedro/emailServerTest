namespace App.Server.Notification.Application.Domain.DataModels.Emailing;

[Serializable]
public record Resource
{
    [JsonConstructor]
    public Resource(NonEmptyString value, NonEmptyString alt, NonEmptyString mediaType)
    {
        Value = value;
        Alt = alt;
        MediaType = mediaType;
    }

    public Resource(NonEmptyString value, NonEmptyString alt)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            MediaType = Path.GetExtension(uri.AbsolutePath);
            Value = value;
            Type = (short)ResourceType.Url;
        }
        else
        {
            var definitions = MimeInspector.Inspector.Inspect(Convert.FromBase64String(value));
            MediaType = definitions.ByMimeType().Single().MimeType;
            Value = value;
            Type = (short)ResourceType.Base64;
        }

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
    public NonEmptyString MediaType { get; init; }

    /// <summary>
    /// The type of the resource.
    /// </summary>
    public short Type { get; init; }
    
    public Attachment ToAttachment()
    {
        var attachment =
            Type == (short)ResourceType.Url
                ? new Attachment(Value, MediaType)
                : new Attachment(new MemoryStream(Convert.FromBase64String(Value)), MediaType);
        
        attachment.TransferEncoding = TransferEncoding.Base64;
        
        return attachment;
    }
}

public enum ResourceType : short
{
    Url,
    Base64
}
