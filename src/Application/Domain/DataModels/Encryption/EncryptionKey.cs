namespace App.Server.Notification.Application.Domain.DataModels.Encryption;

/// <summary>
/// Record for encryption key.
/// </summary>
public record EncryptionKey
{
    /// <summary>
    /// The ID of the encryption key.
    /// </summary>
    public required int KeyId { get; init; }

    /// <summary>
    /// The data of the encryption key.
    /// </summary>
    public required string Data { get; init; }

    /// <summary>
    /// The dat of the encryption key as bytes.
    /// </summary>
    public byte[] DataBytes => Convert.FromBase64String(Data);
}
