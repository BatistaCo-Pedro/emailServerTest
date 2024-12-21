namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// The DTO for updating the SMTP settings of a data owner.
/// </summary>
/// <param name="DataOwnerId">The ID of the data owner to update the smtp settings from.</param>
/// <param name="SmtpSettings">The smtp settings to update to.</param>
public record SmtpSettingsDto(Guid DataOwnerId, SmtpSettings SmtpSettings);
