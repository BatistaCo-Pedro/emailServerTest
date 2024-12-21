namespace App.Server.Notification.Infrastructure.Helpers;

/// <summary>
/// Compressor between strings and byte arrays.
/// </summary>
public static class StringCompressor
{
    private static readonly Encoding CompressionEncoding = Encoding.UTF8;

    /// <summary>
    /// Compresses a <see cref="NonEmptyString"/> to a byte array.
    /// </summary>
    /// <param name="value">The value to compress.</param>
    /// <returns>An array of <see cref="byte"/> with the compressed data.</returns>
    public static byte[] Compress(NonEmptyString value)
    {
        using var compressor = new Compressor();
        return compressor.Wrap(CompressionEncoding.GetBytes(value)).ToArray();
    }

    /// <summary>
    /// Decompresses a byte array to a <see cref="NonEmptyString"/>.
    /// </summary>
    /// <param name="value">The array of <see cref="byte"/> to decompress.</param>
    /// <returns>A <see cref="NonEmptyString"/> decompressed from the byte array.</returns>
    public static NonEmptyString Decompress(byte[] value)
    {
        using var decompressor = new Decompressor();
        return CompressionEncoding.GetString(decompressor.Unwrap(value));
    }
}
