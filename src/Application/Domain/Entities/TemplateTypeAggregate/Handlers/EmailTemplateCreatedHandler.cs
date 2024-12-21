namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate.Handlers;

/// <summary>
/// Event handler for <see cref="EmailTemplateCreated"/>.
/// </summary>
/// <param name="unitOfWork">The unit of work.</param>
/// <remarks>
/// This gets called right before save changes. Do not call <see cref="IUnitOfWork.SaveChanges"/> on here.
/// </remarks>
public class EmailTemplateCreatedHandler(IUnitOfWork unitOfWork)
    : IEventHandler<EmailTemplateCreated>
{
    /// <inheritdoc />
    public Task Handle(EmailTemplateCreated eventMessage)
    {
        if (eventMessage is { DataOwnerId: not null, OnlyEmailTemplateForType: true })
        {
            SetCustomEmailTemplateAsDefault(
                eventMessage.DataOwnerId.Value,
                eventMessage.TemplateTypeId,
                eventMessage.EmailTemplateId
            );
        }

        return Task.CompletedTask;
    }

    private void SetCustomEmailTemplateAsDefault(
        Guid dataOwnerId,
        Guid templateTypeId,
        Guid emailTemplateId
    )
    {
        var dataOwnerRepository = unitOfWork.GetRepository<IDataOwnerRepository>();

        dataOwnerRepository
            .GetById(dataOwnerId)
            .Match(dataOwner =>
            {
                var emailSettings = new EmailSettings(
                    dataOwner.Id,
                    templateTypeId,
                    emailTemplateId
                );

                dataOwner.AddEmailSettings(emailSettings);
                return Result.Ok();
            })
            .ThrowIfFailed();
    }
}
