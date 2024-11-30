namespace App.Server.Notification.Infrastructure.Repositories;

internal class TemplateTypeRepository(NotificationDbContext db)
    : BaseCrudRepository<TemplateType>(db),
        ITemplateTypeRepository
{
    public override IEnumerable<TemplateType> GetByCondition(
        Expression<Func<TemplateType, bool>> condition,
        Func<IQueryable<TemplateType>, IOrderedQueryable<TemplateType>>? orderBy = null,
        string includeProperties = ""
    ) =>
        base.GetByCondition(
            condition,
            orderBy,
            $"{nameof(TemplateType.EmailTemplates)},{includeProperties}"
        );

    public override TemplateType GetById(Guid id) => GetByCondition(x => x.Id == id).First();

    public EmailTemplate GetEmailTemplate(Guid emailTemplateId) =>
        GetByCondition(x => x.EmailTemplates.Any(y => y.Id == emailTemplateId))
            .SelectMany(x => x.EmailTemplates)
            .First(x => x.Id == emailTemplateId);
}
