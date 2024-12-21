namespace App.Server.Notification.Application.Services.Encryption;

/// <summary>
/// Interface for providing encryption keys.
/// </summary>
public interface IEncryptionKeyProvider
{
    /// <summary>
    /// Gets the current encryption key.
    /// </summary>
    /// <returns>The current <see cref="EncryptionKey"/>.</returns>
    EncryptionKey GetCurrentEncryptionKey();

    /// <summary>
    /// Gets the encryption key by the given ID.
    /// </summary>
    /// <param name="keyId">The key ID to get the key by.</param>
    /// <returns>The <see cref="EncryptionKey"/> matching the provided ID.</returns>
    EncryptionKey GetEncryptionKeyById(int keyId);
}

/// <summary>
/// Implementation of the encryption key provider.
/// </summary>
/// <param name="cryptographySettings">The cryptographic settings containing the encryption keys.</param>
/// <remarks>
/// Keys are stored in the app settings file.
/// </remarks>
public class EncryptionKeyProvider(IOptions<CryptographicSettings> cryptographySettings)
    : IEncryptionKeyProvider
{
    private readonly List<EncryptionKey> _encryptionKeys = cryptographySettings
        .Value
        .EncryptionKeys;

    /// <inheritdoc />
    public EncryptionKey GetCurrentEncryptionKey() => _encryptionKeys[^1];

    /// <inheritdoc />
    public EncryptionKey GetEncryptionKeyById(int keyId) => _encryptionKeys[keyId];
}
