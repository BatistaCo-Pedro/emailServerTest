namespace App.Server.Notification.Infrastructure.Persistence.Converters.Compression;

/// <summary>
/// Converts a <see cref="HtmlString"/> to a compressed byte array and vice versa.
/// </summary>
/// <remarks>
/// Due to technical limitations with expression trees every single type to be compressed needs its own converter.
/// </remarks>
internal sealed class CompressedStringConverter : ValueConverter<HtmlString, byte[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompressedStringConverter"/> class.
    /// </summary>
    public CompressedStringConverter()
        : base(
            value => StringCompressor.Compress(value),
            bytes => HtmlString.Create(StringCompressor.Decompress(bytes))
        ) { }
}
