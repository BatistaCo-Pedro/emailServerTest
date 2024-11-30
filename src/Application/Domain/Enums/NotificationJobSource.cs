namespace App.Server.Notification.Application.Domain.Enums;

/// <summary>
/// Source of a notification job.
/// </summary>
/// TODO: Extend and use for auditing
public enum NotificationJobSource : short
{
    Unknown = 0,
    Cms,
}
