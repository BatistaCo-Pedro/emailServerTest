namespace App.Server.Notification.Presentation.Controllers;

/// <summary>
/// Controller for managing settings.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class SettingsController(IDataOwnerService dataOwnerService) : ControllerBase
{
    /// <summary>
    /// Gets a data owner by ID.
    /// </summary>
    /// <param name="dataOwnerId">The ID of the data owner to get.</param>
    [HttpGet]
    public IActionResult GetDataOwner([FromQuery] Guid dataOwnerId) =>
        dataOwnerService.Get(dataOwnerId).Match();

    /// <summary>
    /// Creates a data owner.
    /// </summary>
    /// <param name="dataOwnerDto">The <see cref="DataOwnerDto"/> to use to create a data owner.</param>
    [HttpPost]
    public IActionResult CreateDataOwner([FromBody] DataOwnerDto dataOwnerDto) =>
        dataOwnerService.Create(dataOwnerDto).Match();

    /// <summary>
    /// Updates the smtp settings.
    /// </summary>
    /// <param name="smtpSettingsDto">The <see cref="SmtpSettingsDto"/> to update to.</param>
    [HttpPost]
    public IActionResult UpdateSmtpSettings([FromBody] SmtpSettingsDto smtpSettingsDto) =>
        dataOwnerService.UpdateSmtpSettings(smtpSettingsDto).Match();

    /// <summary>
    /// Updates the default email template.
    /// </summary>
    /// <param name="emailSettingsDto">The <see cref="EmailSettingsDto"/> to update to.</param>
    [HttpPost]
    public IActionResult UpdateDefaultEmailTemplate([FromBody] EmailSettingsDto emailSettingsDto) =>
        dataOwnerService.UpdateDefaultEmailTemplate(emailSettingsDto).Match();
}
