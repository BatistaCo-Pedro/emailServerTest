namespace App.Server.Notification.Application.Abstractions;

public interface ITemplateTypeRepository : ICrudRepository<TemplateType>
{
    EmailTemplate GetEmailTemplate(Guid emailTemplateId);
}