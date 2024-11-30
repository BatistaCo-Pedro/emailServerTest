namespace App.Server.Notification.Infrastructure.Messaging;

/// <summary>
/// Defines settings for connecting to the message broker.
/// </summary>
internal class MessagingSettings
{
    /// <summary>
    /// Host of the message broker.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Port of the message broker.
    /// </summary>
    public ushort Port { get; init; } = 5672;

    /// <summary>
    /// Virtual host name for the message broker.
    /// </summary>
    public required string VirtualHost { get; init; }

    /// <summary>
    /// Username of the broker user.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Password of the broker user.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Whether to use encryption for the broker connection.
    /// </summary>
    public bool SslActive { get; init; }
}
