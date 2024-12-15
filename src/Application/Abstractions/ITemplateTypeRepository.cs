namespace App.Server.Notification.Application.Abstractions;

public interface ITemplateTypeRepository : ICrudRepository<TemplateType>
{
    Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId);
}