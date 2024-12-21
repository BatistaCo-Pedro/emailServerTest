namespace App.Server.Notification.Application.Domain.Entities.DataOwnerAggregate.Handlers;

/// <summary>
/// Handler for when a data owner is created.
/// </summary>
/// <param name="unitOfWork">The unit of work.</param>
/// <remarks>
/// This gets called right before save changes. Do not call <see cref="IUnitOfWork.SaveChanges"/> on here.
/// </remarks>
public class DataOwnerCreatedHandler(IUnitOfWork unitOfWork) : IEventHandler<DataOwnerCreated>
{
    /// <inheritdoc />
    public Task Handle(DataOwnerCreated eventMessage)
    {
        CreateDefaultEmailSettings(eventMessage.DataOwner);

        return Task.CompletedTask;
    }

    private void CreateDefaultEmailSettings(DataOwner dataOwner)
    {
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        var templateTypes = templateTypeRepository.GetAll().ToList();

        AddEmailSettingsToDataOwner(dataOwner, templateTypes).ThrowIfFailed();
    }

    private static Result AddEmailSettingsToDataOwner(
        DataOwner dataOwner,
        List<TemplateType> templateTypes
    )
    {
        foreach (var templateType in templateTypes)
        {
            if (templateType.EmailTemplates.FirstOrDefault() is { } emailTemplate)
            {
                var emailSettings = new EmailSettings(
                    dataOwner.Id,
                    templateType.Id,
                    emailTemplate.Id
                );

                // ATTENTION: this only works because the ChangeTracker tracks references
                dataOwner.AddEmailSettings(emailSettings);
            }
        }

        return Result.Ok();
    }
}
