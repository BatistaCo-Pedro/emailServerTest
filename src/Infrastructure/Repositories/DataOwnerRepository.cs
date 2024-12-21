namespace App.Server.Notification.Infrastructure.Repositories;

/// <summary>
/// Repository for <see cref="DataOwner"/> entities.
/// </summary>
/// <param name="db">The database context.</param>
internal class DataOwnerRepository(NotificationDbContext db)
    : BaseCrudRepository<DataOwner>(db),
        IDataOwnerRepository
{
    private readonly NotificationDbContext _db = db;

    /// <inheritdoc />
    public override IEnumerable<DataOwner> GetByCondition(
        Expression<Func<DataOwner, bool>> condition,
        Func<IQueryable<DataOwner>, IOrderedQueryable<DataOwner>>? orderBy = null,
        string includeProperties = ""
    ) =>
        base.GetByCondition(
            condition,
            orderBy,
            $"{nameof(DataOwner.EmailSettings)},{includeProperties}"
        );

    /// <inheritdoc />
    public override Result<DataOwner> GetById(Guid id) =>
        GetByCondition(x => x.Id == id)
            .FirstOrDefault()
            .ToResult($"{nameof(DataOwner)} with ID: `{id}` not found.");

    /// <inheritdoc />
    public Result<DataOwner> GetByUniqueProperties(string name, string source) =>
        base.GetByCondition(x => x.Name == name && x.Source == source)
            .FirstOrDefault()
            .ToResult($"Couldn't find a data owner with the name {name} and source {source}");

    /// <inheritdoc />
    public IEnumerable<EmailSettings> GetEmailSettingsByEmailTemplateId(Guid emailTemplateId)
    {
        return _db.Set<EmailSettings>().Where(x => x.DefaultEmailTemplateId == emailTemplateId);
    }

    /// <inheritdoc />
    public Result FallBackOrDeleteEmailSettings(
        Guid emailTemplateId,
        Result<EmailTemplate> fallBackEmailTemplate
    )
    {
        var emailSettingsList = _db.Set<EmailSettings>()
            .Where(x => x.DefaultEmailTemplateId == emailTemplateId)
            .ToList();

        return fallBackEmailTemplate.Match(
            value => SetFallbackEmailTemplate(emailSettingsList, value),
            _ => DeleteEmailSettings(emailSettingsList)
        );
    }

    private static Result SetFallbackEmailTemplate(
        List<EmailSettings> emailSettingsList,
        EmailTemplate fallBackEmailTemplate
    )
    {
        foreach (var emailSettings in emailSettingsList)
        {
            emailSettings.ReplaceDefaultEmailTemplate(fallBackEmailTemplate.Id);
        }

        return Result.Ok();
    }

    private Result DeleteEmailSettings(List<EmailSettings> emailSettingsList)
    {
        foreach (var emailSettings in emailSettingsList)
        {
            var entry = db.Set<EmailSettings>().Remove(emailSettings);

            if (entry.State != EntityState.Deleted)
            {
                return Result.Fail($"Couldn't delete email settings with ID {emailSettings.Id}.");
            }
        }

        return Result.Ok();
    }
}
