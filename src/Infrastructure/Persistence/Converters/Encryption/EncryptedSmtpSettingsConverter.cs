namespace App.Server.Notification.Infrastructure.Persistence.Converters.Encryption;

/// <summary>
/// Converts a <see cref="SmtpSettings"/> to an encrypted string and vice versa.
/// </summary>
internal sealed class EncryptedSmtpSettingsConverter : ValueConverter<SmtpSettings, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedSmtpSettingsConverter"/> class.
    /// </summary>
    public EncryptedSmtpSettingsConverter()
        : base(
            value =>
                AesEncryptionHelper.Encrypt(
                    JsonSerializer.Serialize(value, new JsonSerializerOptions())
                ),
            cipherText =>
                JsonSerializer.Deserialize<SmtpSettings>(
                    AesEncryptionHelper.Decrypt(cipherText),
                    new JsonSerializerOptions()
                )!
        ) { }
}
