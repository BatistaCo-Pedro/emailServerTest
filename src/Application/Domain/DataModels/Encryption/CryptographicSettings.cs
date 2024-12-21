namespace App.Server.Notification.Application.Domain.DataModels.Encryption;

/// <summary>
/// Record for containing settings.
/// </summary>
public record CryptographicSettings
{
    /// <summary>
    /// The encryption keys contained within the cryptographic settings.
    /// </summary>
    public required List<EncryptionKey> EncryptionKeys { get; init; }
}
