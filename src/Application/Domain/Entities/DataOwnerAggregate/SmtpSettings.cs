namespace App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate;

/// <summary>
/// Record representing SMTP settings.
/// </summary>
/// <remarks>
/// This class also gets used to save SMTP settings in the app settings.
/// That's the reason why the properties are from system types and not domain types.
/// E.g. <see cref="string"/> instead of <see cref="NonEmptyString"/>.
/// </remarks>
[Serializable]
public record SmtpSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpSettings"/> class.
    /// </summary>
    /// <remarks>
    /// Constructor used by the configuration section reader to get the settings from the app settings JSON file.
    /// </remarks>
    [Obsolete("Required by configuration section reader - use a different constructor.")]
    public SmtpSettings() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpSettings"/> class.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    /// <param name="address">The sender address.</param>
    /// <param name="password">The password.</param>
    /// <param name="deliveryMethod">The delivery method.</param>
    /// <param name="enableSsl">Whether SSL is enabled.</param>
    [SetsRequiredMembers]
    [JsonConstructor]
    public SmtpSettings(
        NonEmptyString host,
        int port,
        NonEmptyString address,
        NonEmptyString? password,
        SmtpDeliveryMethod deliveryMethod,
        bool enableSsl
    )
    {
        Host = host;
        Port = port;
        Address = address;
        Password = password?.Value;
        DeliveryMethod = deliveryMethod.ToString();
        EnableSsl = enableSsl;
    }

    /// <summary>
    /// The host.
    /// </summary>
    [JsonPropertyName("host")]
    public required string Host { get; init; }

    /// <summary>
    /// The port.
    /// </summary>
    [JsonPropertyName("port")]
    public required int Port { get; init; }

    /// <summary>
    /// The sender address.
    /// </summary>
    [JsonPropertyName("address")]
    public required string Address { get; init; }

    /// <summary>
    /// The password.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; init; }

    /// <summary>
    /// The delivery method.
    /// </summary>
    [JsonPropertyName("deliveryMethod")]
    public string DeliveryMethod { get; init; } = SmtpDeliveryMethod.Network.ToString();

    /// <summary>
    /// Whether SSL is enabled.
    /// </summary>
    [JsonPropertyName("enableSsl")]
    public required bool EnableSsl { get; init; }
}
