namespace App.Server.Notification.Application.Domain.DataModels.Encryption;

/// <summary>
/// Record for AES GCM ciphertext.
/// </summary>
/// <param name="Nonce">The nonce of the cipher text.</param>
/// <param name="Tag">The tag of the cipher text.</param>
/// <param name="CipherTextBytes">The cipher text in bytes.</param>
/// <param name="KeyId">The key ID.</param>
public record AesGcmCiphertext(byte[] Nonce, byte[] Tag, byte[] CipherTextBytes, int KeyId)
{
    /// <summary>
    /// The separator between the key ID and the cipher text.
    /// </summary>
    private const char Separator = '$';

    /// <summary>
    /// Creates an AES GCM cipher text from a base64 string.
    /// </summary>
    /// <param name="data">The base 64 string.</param>
    /// <returns>A <see cref="AesGcmCiphertext"/> object.</returns>
    public static AesGcmCiphertext FromBase64String(NonEmptyString data)
    {
        var parts = data.Value.Split(Separator);

        var keyId = int.Parse(parts[0]);
        var cipherText = parts[1];

        var dataBytes = Convert.FromBase64String(cipherText);

        return new AesGcmCiphertext(
            dataBytes.Take(AesGcm.NonceByteSizes.MaxSize).ToArray(),
            dataBytes[^AesGcm.TagByteSizes.MaxSize..],
            dataBytes[AesGcm.NonceByteSizes.MaxSize..^AesGcm.TagByteSizes.MaxSize],
            keyId
        );
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{KeyId}{Separator}{Convert.ToBase64String(Nonce.Concat(CipherTextBytes).Concat(Tag).ToArray())}";
}
