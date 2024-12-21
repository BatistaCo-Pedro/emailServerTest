namespace App.Server.Notification.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="ITemplateTypeRepository"/>.
/// </summary>
/// <param name="db">The database context.</param>
internal class TemplateTypeRepository(NotificationDbContext db)
    : BaseCrudRepository<TemplateType>(db),
        ITemplateTypeRepository
{
    /// <inheritdoc />
    public override IEnumerable<TemplateType> GetByCondition(
        Expression<Func<TemplateType, bool>> condition,
        Func<IQueryable<TemplateType>, IOrderedQueryable<TemplateType>>? orderBy = null,
        string includeProperties = ""
    ) =>
        base.GetByCondition(
            condition,
            orderBy,
            $"{nameof(TemplateType.EmailTemplates)}.{nameof(EmailTemplate.EmailBodyContents)},{includeProperties}"
        );

    /// <inheritdoc />
    public override Result<TemplateType> GetById(Guid id) =>
        GetByCondition(x => x.Id == id)
            .FirstOrDefault()
            .ToResult($"TemplateType with ID: `{id}` not found.");

    /// <inheritdoc />
    public Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId) =>
        GetByCondition(x => x.EmailTemplates.Any(y => y.Id == emailTemplateId))
            .SelectMany(x => x.EmailTemplates)
            .FirstOrDefault(x => x.Id == emailTemplateId)
            .ToResult($"EmailTemplate with ID: `{emailTemplateId}` not found.");

    /// <inheritdoc />
    public Result DeleteEmailTemplate(EmailTemplate emailTemplate)
    {
        // this needs to be called so domain specific rules and possible domain events get handled correctly
        emailTemplate.TemplateType.RemoveEmailTemplate(emailTemplate);

        return db.Set<EmailTemplate>().Remove(emailTemplate).State == EntityState.Deleted
            ? Result.Ok()
            : Result.Fail($"Couldn't delete email template with id {emailTemplate.Id}");
    }
}
