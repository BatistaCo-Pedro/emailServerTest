namespace App.Server.Notification.Presentation.Controllers;

/// <summary>
/// Controller for managing email templates.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class EmailTemplateController(IEmailTemplateService emailTemplateService) : ControllerBase
{
    /// <summary>
    /// Gets an email template.
    /// </summary>
    /// <param name="templateTypeId">The ID of the email template to get.</param>
    [HttpGet]
    public IActionResult Get([FromQuery] Guid templateTypeId) =>
        emailTemplateService.Get(templateTypeId).Match();

    /// <summary>
    /// Creates an email template.
    /// </summary>
    /// <param name="createEmailTemplateDto">The <see cref="CreateEmailTemplateDto"/> to use to create a new email template.</param>
    [HttpPost]
    public IActionResult Create([FromBody] CreateEmailTemplateDto createEmailTemplateDto) =>
        emailTemplateService.Create(createEmailTemplateDto).Match();

    /// <summary>
    /// Updates the content of an email template.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template to update.</param>
    /// <param name="emailBodyContentDto">The <see cref="CreateEmailBodyContentDto"/> to use to update the content.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult UpdateContent(
        [FromQuery] Guid emailTemplateId,
        [FromBody] CreateEmailBodyContentDto emailBodyContentDto
    ) => emailTemplateService.UpdateContent(emailTemplateId, emailBodyContentDto).Match();
}
