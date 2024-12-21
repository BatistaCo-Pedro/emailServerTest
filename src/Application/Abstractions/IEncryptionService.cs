namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Service for encrypting and decrypting data.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the given data.
    /// </summary>
    /// <param name="dataToEncrypt">The data to encrypt.</param>
    /// <returns>The encrypted data as string.</returns>
    string Encrypt(NonEmptyString dataToEncrypt);

    /// <summary>
    /// Decrypts the given ciphertext.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <returns>The decrypted text as string.</returns>
    string Decrypt(NonEmptyString cipherText);
}
