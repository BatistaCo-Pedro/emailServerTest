using System.Collections.Immutable;
using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using App.Server.Notification.Application.Domain.DataModels.Emailing;
using App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate;
using App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;

namespace App.Server.Notification.Presentation;

[ApiController]
public class TestController(
    IEmailTemplateService emailTemplateService,
    ITemplateTypeService templateTypeService,
    IDataOwnerService dataOwnerService,
    IUnitOfWork unitOfWork,
    IOptions<SmtpSettings> defaultSmtpSettings,
    IEmailSender emailSender
//IMailingQueue mailingQueue
) : ControllerBase
{
    [HttpGet]
    [Route("/get")]
    public IActionResult Get()
    {
        var templateTypeRepo = unitOfWork.GetRepository<ITemplateTypeRepository>();

        var allTemplateTypes = templateTypeRepo.GetAll();

        foreach (var emailTemplate in allTemplateTypes.SelectMany(x => x.EmailTemplates).ToList())
        {
            emailTemplateService.Delete(emailTemplate.Id);
        }

        var createEmailBodyContentDto = new CreateEmailBodyContentDto(
            "en-Us",
            "Subject",
            Json.RootElement.GetRawText(),
            Json,
            true
        );

        var createEmailBodyContentDto2 = new CreateEmailBodyContentDto(
            "it-It",
            "Subjecti",
            Json.RootElement.GetRawText(),
            Json,
            true
        );

        ImmutableHashSet<MergeTag> acceptedMergeTags =
        [
            new("name1", typeof(double)),
            new("name2", typeof(string)),
            new("name3", typeof(DateOnly)),
            new("name4", typeof(TimeOnly)),
            new("name5", typeof(Guid)),
        ];

        var templateTypeId = Guid.Parse("019395a6-11fe-0fcb-2905-bfc842a2c218");

        var createEmailTemplateDto = new CreateEmailTemplateDto(
            "name",
            createEmailBodyContentDto,
            templateTypeId
        );

        var emailTemplateId = emailTemplateService.Create(createEmailTemplateDto);

        emailTemplateService.UpdateContent(emailTemplateId.Value, createEmailBodyContentDto2);

        return Ok();
    }

    [HttpGet]
    [Route("/get2")]
    public IActionResult Get2()
    {
        var templateTypeRepo = unitOfWork.GetRepository<ITemplateTypeRepository>();
        var templateType = templateTypeRepo
            .GetById(Guid.Parse("0193922b-ab00-3504-ba73-8fc8a0fb2e90"))
            .Value;
        var emailTemplate = templateType.EmailTemplates.First();

        var createEmailBodyContentDto = new CreateEmailBodyContentDto(
            "it-It",
            "Subjecti",
            Json2.RootElement.GetRawText(),
            Json2,
            true
        );

        var emailBodyContent = createEmailBodyContentDto.ToEntity(templateType.AcceptedMergeTags);

        emailTemplate.UpdateContent(emailBodyContent);

        unitOfWork.SaveChanges();

        return Ok();
    }

    [HttpGet]
    [Route("/get3")]
    public IActionResult Get3()
    {
        var templateType = new TemplateTypeDto("somethin", []);

        templateTypeService.Create(templateType);

        return Ok();
    }

    [HttpGet]
    [Route("testDataOwnerCreated")]
    public IActionResult TestDataOwnerCreated()
    {
        var createEmailBodyContentDto = new CreateEmailBodyContentDto(
            "en-Us",
            "Subject",
            Json.RootElement.GetRawText(),
            Json,
            true
        );

        var templateType1 = new TemplateTypeDto("somethinadadad", []);

        templateTypeService.Create(templateType1);

        var templateType2 = new TemplateTypeDto("somethinadadadadadad2", []);

        var templateTypeId2 = templateTypeService.Create(templateType2);

        unitOfWork.SaveChanges();

        var createEmailTemplateDto = new CreateEmailTemplateDto(
            "nameasdfaf",
            createEmailBodyContentDto,
            templateTypeId2
        );

        emailTemplateService.Create(createEmailTemplateDto);

        unitOfWork.SaveChanges();

        var templateType3 = new TemplateTypeDto("somethinadadadadadad3", []);

        var templateTypeId3 = templateTypeService.Create(templateType3);

        unitOfWork.SaveChanges();

        var createEmailTemplateDto2 = new CreateEmailTemplateDto(
            "namessssss",
            createEmailBodyContentDto,
            templateTypeId3
        );

        emailTemplateService.Create(createEmailTemplateDto2);

        unitOfWork.SaveChanges();

        var dataOwner = new DataOwnerDto("some-name", "some-source", []);

        dataOwnerService.Create(dataOwner);

        return Ok();
    }

    [HttpGet]
    [Route("testEmailDeleted")]
    public IActionResult TestEmailTemplateDeleted()
    {
        unitOfWork.UseTransaction(TestEmailTemplateDeletedImpl);
        return Ok();
    }

    private void TestEmailTemplateDeletedImpl()
    {
        var dataOwner = new DataOwnerDto("some-nasdadadame", "some-souadadadrce", []);

        var dataOwnerId = dataOwnerService.Create(dataOwner).Value;

        unitOfWork.SaveChanges();

        var createEmailBodyContentDto = new CreateEmailBodyContentDto(
            "en-Us",
            "Subject",
            Json.RootElement.GetRawText(),
            Json,
            true
        );

        var templateTypeDto = new TemplateTypeDto("this is a test OMG", []);

        var templateTypeId = templateTypeService.Create(templateTypeDto);

        var createEmailTemplateDto = new CreateEmailTemplateDto(
            "emailTemplateTest OMG",
            createEmailBodyContentDto,
            templateTypeId,
            DataOwnerId: dataOwnerId
        );

        var createEmailTemplateDto2 = new CreateEmailTemplateDto(
            "Fallback to this one",
            createEmailBodyContentDto,
            templateTypeId,
            DataOwnerId: dataOwnerId
        );

        var emailTemplateId = emailTemplateService.Create(createEmailTemplateDto);
        // var emailTemplateId2 = emailTemplateService.Create(createEmailTemplateDto2);

        unitOfWork.SaveChanges();

        emailTemplateService.Delete(emailTemplateId.Value);
    }

    [HttpGet]
    [Route("testCreateDataOwner")]
    public IActionResult TestCreateDataOwner()
    {
        var allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var dataOwnerDto = new DataOwnerDto(
            RandomNumberGenerator.GetString(allChars, 10),
            RandomNumberGenerator.GetString(allChars, 10),
            []
        );

        var id = dataOwnerService.Create(dataOwnerDto).Value;

        var dataOwnerRepository = unitOfWork.GetRepository<IDataOwnerRepository>();

        var dataOwner = dataOwnerRepository.GetById(id).Value;

        var newSmtpSettings = new SmtpSettings(
            "mail.google.com",
            587,
            "lol@wnd.ch",
            "Bfo12345",
            SmtpDeliveryMethod.Network,
            true
        );

        dataOwner.UpdateSmtpSettings(newSmtpSettings);

        unitOfWork.SaveChanges();

        return Ok();
    }

    [HttpGet]
    [Route("getDataOwnerWIthSmtpSettingsTest")]
    public IActionResult GetDataOwnerWIthSmtpSettingsTest()
    {
        var dataOwnerRepository = unitOfWork.GetRepository<IDataOwnerRepository>();

        var dataOwner = dataOwnerRepository
            .GetByCondition(x => x.SmtpSettings != null)
            .FirstOrDefault();

        var smtpSettings = defaultSmtpSettings.Value;

        return Ok();
    }

    [HttpGet]
    [Route("ultimate")]
    public IActionResult Ultimate()
    {
        var dataOwnerId = Guid.Empty;

        var templateTypeId = Guid.Empty;

        /*unitOfWork.UseTransaction(() =>
        {
            HashSet<MergeTag> acceptedMergeTags =
            [
                new("name1", typeof(double)),
                new("name2", typeof(string)),
                new("time", typeof(TimeOnly)),
                new("dateTime", typeof(DateTime)),
            ];

            HashSet<CustomMergeTagDto> data = [new("name4", "value5"), new("name5", "5")];
            
            var dataOwnerDto = new DataOwnerDto(
                "some-namessss",
                "some-source",
                data.ToImmutableHashSet()
            );
            
            var dataOwnerDto2 = new DataOwnerDto(
                "some-name-2sss",
                "some-source",
                data.ToImmutableHashSet()
            );

            dataOwnerId = dataOwnerService.Create(dataOwnerDto).Value;
            var dataOwnerId2 = dataOwnerService.Create(dataOwnerDto2).Value;

            var templateTypeDto = new TemplateTypeDto(
                "book-confssss",
                acceptedMergeTags.ToImmutableHashSet()
            );

            templateTypeId = templateTypeService.Create(templateTypeDto).Value;

            var emailBodyContentDto = new CreateEmailBodyContentDto(
                "en-Us",
                "Subject something",
                Json.RootElement.GetRawText(),
                Json,
                true
            );

            var emailTemplateDto = new CreateEmailTemplateDto(
                "name soethi sada idk ASsS",
                emailBodyContentDto,
                templateTypeId
            );

            var emailTemplateId = emailTemplateService.Create(emailTemplateDto).Value;
        });*/

        var sender = new MailAddress("feedback@mywnd.tv", "WND CH");
        var recipient = new MailAddress("pb@wnd.ch", "Pedro");
        var emailInfo = new EmailInfoDto(
            Guid.Parse("0193ded4-425b-3c87-30fd-7cae7fb0ad1c"),
            Guid.Parse("0193ded4-449d-1093-1d55-0b30ccee47bc"),
            sender.DisplayName,
            sender.Address,
            recipient.DisplayName,
            recipient.Address,
            "en-Us",
            new Dictionary<string, string>()
            {
                { "name1", DateTime.UtcNow.ToString() },
                { "name2", TimeOnly.MaxValue.ToString() },
                { "name3", Guid.NewGuid().ToString() },
            }.ToImmutableDictionary(),
            []
        );

        emailSender.SendEmail(emailInfo);

        //mailingQueue.EnqueueEmail(emailInfo);

        return Ok();
    }

    [HttpGet("[action]")]
    public IActionResult TestSerializationFromDic()
    {
        var emailInfo = new EmailInfoDto(
            Guid.Parse("0193ded4-425b-3c87-30fd-7cae7fb0ad1c"),
            Guid.Parse("0193ded4-449d-1093-1d55-0b30ccee47bc"),
            "test",
            "test@test-test.test",
            "test",
           "test@test-test.test",
            "en-Us",
            new Dictionary<string, string>()
            {
                { "name1", DateTime.UtcNow.ToString() },
                { "name2", TimeOnly.MaxValue.ToString() },
                { "name3", Guid.NewGuid().ToString() },
            }.ToImmutableDictionary(),
            []
        );

        var json = JsonSerializer.Serialize(emailInfo);
        var t = JsonSerializer.Deserialize<EmailInfoDto>(json);
        
        return Ok();
    }

    private static readonly JsonDocument Json = JsonDocument.Parse(
        """
        {
            "item1": {
                "text": "{{name1}}"
            },
            "item2": {
                "text": "{{name2}}",
                "item3": {
                   "text": "{{name3}}",
                   "item4": {
                      "text": "{{name4}}"
                  }
               }
            },
            "item5": {
                "text": "{{name5}}"
            },
            "item6": {
                "text": "{{name6}}"
            },
            "item7": {
              "text": "{{name7}}"
          }
        }
        """
    );

    private static readonly JsonDocument Json2 = JsonDocument.Parse(
        Json.RootElement.GetRawText().Remove(236 - 1, 50)
    );
}
