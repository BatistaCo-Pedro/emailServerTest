namespace App.Server.Notification.Presentation.Controllers;

/// <summary>
/// Controller for managing template types.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class TemplateTypeController(ITemplateTypeService templateTypeService) : ControllerBase
{
    /// <summary>
    /// Creates a template type.
    /// </summary>
    /// <param name="templateTypeDto">The <see cref="TemplateTypeDto"/> to use to create a new template type.</param>
    [HttpPost]
    public IActionResult Create([FromBody] TemplateTypeDto templateTypeDto) =>
        templateTypeService.Create(templateTypeDto).Match();

    /// <summary>
    /// Gets all the template types.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(templateTypeService.GetAll());

    /// <summary>
    /// Gets all email templates belonging to a template type.
    /// </summary>
    /// <param name="templateTypeId">The ID of the template type to get the email templates for.</param>
    [HttpGet]
    public IActionResult Get([FromQuery] Guid templateTypeId) =>
        templateTypeService.GetEmailTemplates(templateTypeId).Match();
}
