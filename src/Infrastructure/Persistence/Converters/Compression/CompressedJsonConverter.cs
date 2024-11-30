namespace App.Server.Notification.Infrastructure.Persistence.Converters.Compression;

/// <summary>
/// Converts a <see cref="JsonDocument"/> to a compressed byte array and vice versa.
/// </summary>
/// <remarks>
/// Due to technical limitations with expression trees every single type to be compressed needs its own converter.
/// </remarks>
internal sealed class CompressedJsonConverter : ValueConverter<JsonDocument, byte[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompressedJsonConverter"/> class.
    /// </summary>
    public CompressedJsonConverter()
        : base(
            value => StringCompressor.Compress(value.RootElement.GetRawText()),
            bytes =>
                JsonDocument.Parse(StringCompressor.Decompress(bytes), new JsonDocumentOptions())
        ) { }
}
