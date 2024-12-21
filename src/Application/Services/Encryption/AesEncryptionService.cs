namespace App.Server.Notification.Application.Services.Encryption;

/// <summary>
/// Aes implementation of the encryption service.
/// </summary>
/// <param name="encryptionKeyProvider">The encryption key provider.</param>
public class AesEncryptionService(IEncryptionKeyProvider encryptionKeyProvider) : IEncryptionService
{
    /// <inheritdoc />
    public string Encrypt(NonEmptyString dataToEncrypt)
    {
        var key = encryptionKeyProvider.GetCurrentEncryptionKey();

        using var aes = new AesGcm(key.DataBytes, AesGcm.TagByteSizes.MaxSize);

        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        var plaintextBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
        var ciphertextBytes = new byte[plaintextBytes.Length];

        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aes.Encrypt(nonce, plaintextBytes, ciphertextBytes, tag);
        return new AesGcmCiphertext(nonce, tag, ciphertextBytes, key.KeyId).ToString();
    }

    /// <inheritdoc />
    public string Decrypt(NonEmptyString cipherText)
    {
        var gcmCiphertext = AesGcmCiphertext.FromBase64String(cipherText);

        var key = encryptionKeyProvider.GetEncryptionKeyById(gcmCiphertext.KeyId);

        using var aes = new AesGcm(key.DataBytes, AesGcm.TagByteSizes.MaxSize);

        var plaintextBytes = new byte[gcmCiphertext.CipherTextBytes.Length];

        aes.Decrypt(
            gcmCiphertext.Nonce,
            gcmCiphertext.CipherTextBytes,
            gcmCiphertext.Tag,
            plaintextBytes
        );

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
