namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// A static wrapper around the AES encryption service.
/// </summary>
public static class AesEncryptionHelper
{
    /// <summary>
    /// The aes encryption service to use.
    /// </summary>
    /// <remarks>
    /// This fields must be set before using the helper.
    /// </remarks>
    private static AesEncryptionService? _encryptionService;

    /// <summary>
    /// Sets the encryption service to use.
    /// </summary>
    /// <param name="aesEncryptionService">The aes encryption service to use.</param>
    public static void SetEncryptionService(AesEncryptionService aesEncryptionService)
    {
        _encryptionService = aesEncryptionService;
    }

    /// <summary>
    /// Encrypts the given data.
    /// </summary>
    /// <param name="dataToEncrypt">The data to encrypt.</param>
    /// <returns>The encrypted data as string.</returns>
    public static NonEmptyString Encrypt(NonEmptyString dataToEncrypt)
    {
        ArgumentNullException.ThrowIfNull(_encryptionService, nameof(_encryptionService));
        return _encryptionService.Encrypt(dataToEncrypt);
    }

    /// <summary>
    /// Decrypts the given ciphertext.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <returns>The decrypted text as string.</returns>
    public static NonEmptyString Decrypt(NonEmptyString cipherText)
    {
        ArgumentNullException.ThrowIfNull(_encryptionService, nameof(_encryptionService));
        return _encryptionService.Decrypt(cipherText);
    }
}
