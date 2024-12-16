using System.Collections.Immutable;

namespace App.Server.Notification.Presentation.Controllers;

[ApiController]
public class TestController(IMailingQueue mailingQueue, IMailingQueueViewer mailingQueueViewer) : ControllerBase
{
    [HttpGet]
    [Route("api/TestEnqueueEmail")]
    public IActionResult TestEnqueueEmail()
    {
        var guid = Guid.NewGuid();
        
        var emailRequestDto = new EmailRequestDto(
            Guid.NewGuid(),
            "en-US",
            "Sender Name",
            "test@test.ccccc",
            "Recipient Name",
            "rec@test.cccccc",
            new Dictionary<string, object>().ToImmutableDictionary(),
            guid
        );

        mailingQueue.EnqueueEmail(emailRequestDto);

        mailingQueueViewer.PeekEmail(guid, out var dto);

        return Ok();
    }

    [HttpGet]
    [Route("api/TestScheduledEmail")]
    public IActionResult TestScheduledEmail()
    {
        var guid = Guid.NewGuid();
        
        var emailRequestDto = new EmailRequestDto(
            Guid.NewGuid(),
            "en-US",
            "Sender Name",
            "test@test.ccccc",
            "Recipient Name",
            "rec@test.cccccc",
            new Dictionary<string, object>().ToImmutableDictionary(),
            guid
        );
        
        var scheduledEmailRequestDto = new ScheduledEmailRequestDto(emailRequestDto, TimeSpan.FromSeconds(10));

        mailingQueue.EnqueueScheduledEmail(scheduledEmailRequestDto);
        
        mailingQueueViewer.PeekEmail(guid, out var dto);

        return Ok();
    }
}
