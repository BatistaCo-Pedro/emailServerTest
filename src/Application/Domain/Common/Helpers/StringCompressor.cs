namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Static compressor between strings and byte arrays.
/// </summary>
public static class StringCompressor
{
    private static readonly Encoding CompressionEncoding = Encoding.UTF8;

    /// <summary>
    /// Compresses a non-empty string into a byte array.
    /// </summary>
    /// <param name="value">The <see cref="NonEmptyString"/> to compress.</param>
    /// <returns>A <see cref="byte"/>[] with the compressed data.</returns>
    public static byte[] Compress(NonEmptyString value)
    {
        using var compressor = new Compressor();
        return compressor.Wrap(CompressionEncoding.GetBytes(value)).ToArray();
    }

    /// <summary>
    /// Decompresses a <see cref="byte"/>[] into a string.
    /// </summary>
    /// <param name="value">The <see cref="byte"/>[] to decompress.</param>
    /// <returns>The decompressed data as a <see cref="NonEmptyString"/>.</returns>
    public static NonEmptyString Decompress(byte[] value)
    {
        using var decompressor = new Decompressor();
        return CompressionEncoding.GetString(decompressor.Unwrap(value));
    }
}
