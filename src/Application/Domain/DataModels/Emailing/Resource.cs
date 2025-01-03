namespace App.Server.Notification.Application.Domain.DataModels.Emailing;

/// <summary>
/// Custom type converter for <see cref="Resource"/>.
/// </summary>
public class ResourceTypeConverter : TypeConverter
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
}

[Serializable]
[TypeConverter(typeof(ResourceTypeConverter))]
public abstract record Resource : IParsable<Resource>
{
    private static readonly ImmutableHashSet<string> UnsupportedMimeTypes =
    [
        MediaTypeNames.Image.Svg // unsafe
    ];

    [JsonConstructor]
    protected Resource(NonEmptyString value, NonEmptyString alt, NonEmptyString mimeType)
    {
        Value = value;
        Alt = alt;
        MimeType = mimeType;
        ThrowIfUnsupportedMimeType();
    }

    public static Resource Create(NonEmptyString value, NonEmptyString alt) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
            ? new UriResource(uri, alt)
            : new Base64Resource(new Base64String(value), alt);

    /// <summary>
    /// Either a Base64 encoded string or a URL to the resource.
    /// </summary>
    [JsonPropertyName("value")]
    public NonEmptyString Value { get; init; }

    /// <summary>
    /// The alt value to use in case the resource can not be loaded.
    /// </summary>
    [JsonPropertyName("alt")]
    public NonEmptyString Alt { get; init; }

    /// <summary>
    /// The media type of the resource.
    /// </summary>
    [JsonPropertyName("miType")]
    public NonEmptyString MimeType { get; init; }

    /// <summary>
    /// Gets the stream to build the resource.
    /// </summary>
    /// <returns>The stream with the content for the resources.</returns>
    protected abstract Stream GetStream();

    /// <summary>
    /// Creates a <see cref="Attachment"/> from the resource.
    /// </summary>
    /// <returns>A <see cref="Attachment"/> from the resource.</returns>
    public Attachment ToAttachment(NonEmptyString name) =>
        new(GetStream(), name, MimeType) { TransferEncoding = TransferEncoding.Base64 };

    /// <summary>
    /// Creates a <see cref="LinkedResource"/> from the resource.
    /// </summary>
    /// <param name="contentId">The content ID to set.</param>
    /// <returns>A <see cref="LinkedResource"/> from the resource.</returns>
    public LinkedResource ToLinkedResource(NonEmptyString contentId) =>
        IsImage()
            ? new LinkedResource(GetStream(), MimeType)
            {
                TransferEncoding = TransferEncoding.Base64,
                ContentId = contentId
            }
            : throw new ArgumentException("Linked resources must be images.");
    
    /// <inheritdoc />
    public static Resource Parse(string s, IFormatProvider? provider) => Create(s, string.Empty);
    
    /// <inheritdoc />
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Resource result
    )
    {
        if(s is null)
        {
            result = null;
            return false;
        }
        
        result = Parse(s, provider);
        return true;
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    protected bool IsImage() => MimeType.Value.StartsWith("image");

    protected bool IsUnsupportedMimeType() => UnsupportedMimeTypes.Contains(MimeType);

    protected void ThrowIfUnsupportedMimeType()
    {
        if (IsUnsupportedMimeType())
        {
            throw new ArgumentException($"The MIME type {MimeType} is not supported.");
        }
    }
}

public record UriResource : Resource
{
    private static readonly HttpClient Client =
        new(new SocketsHttpHandler() { PooledConnectionLifetime = TimeSpan.FromMinutes(10), })
        {
            Timeout = TimeSpan.FromSeconds(10),
        };

    public UriResource(Uri uri, NonEmptyString alt)
        : base(uri.AbsolutePath, alt, MimeTypeHelper.GetMimeType(uri))
    {
        if (!uri.IsWellFormedOriginalString())
        {
            throw new ArgumentException("The URI is not well formed.");
        }
    }

    protected override Stream GetStream() => Client.GetStreamAsync(Value).Result;
}

public record Base64Resource : Resource
{
    public Base64Resource(Base64String value, NonEmptyString alt)
        : base(value, alt, MimeTypeHelper.GetMimeType(value)) { }

    protected override Stream GetStream() => new MemoryStream(Convert.FromBase64String(Value));
}
