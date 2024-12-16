namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing email presets.
/// </summary>
public interface IEmailPresetService
{
    /// <summary>
    /// Creates an email preset.
    /// </summary>
    /// <param name="emailPresetDto">The email preset DTO containing the information to create an email preset.</param>
    /// <returns>A <see cref="Result"/> containing the ID of the newly created email preset.</returns>
    Result<Guid> Create(EmailPresetDto emailPresetDto);

    /// <summary>
    /// Deletes an email template and its presets.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template the preset belongs to.</param>
    /// <param name="emailPresetId">The ID of the email preset to delete.</param>
    /// <returns>A <see cref="Result"/> defining the success of the operation.</returns>
    public Result Delete(Guid emailTemplateId, Guid emailPresetId);
}

/// <inheritdoc />
public class EmailPresetService(IUnitOfWork unitOfWork) : IEmailPresetService
{
    private readonly ITemplateTypeRepository _templateTypeRepository =
        unitOfWork.GetRepository<ITemplateTypeRepository>();

    /// <inheritdoc />
    public Result<Guid> Create(EmailPresetDto emailPresetDto)
    {
        return Result<Guid>
            .Try(() =>
            {
                var emailPreset = emailPresetDto.ToEntity();

                var emailTemplate = _templateTypeRepository.GetEmailTemplate(
                    emailPresetDto.EmailTemplateId
                ).Value;

                emailTemplate.AddPreset(emailPreset);

                unitOfWork.SaveChanges();

                return emailPreset.Id;
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(Create),
                "Email preset with ID {EmailPresetId} created successfully",
                result => [result.Value]
            );
    }

    /// <inheritdoc />
    public Result Delete(Guid emailTemplateId, Guid emailPresetId)
    {
        return Result
            .Try(() =>
            {
                var emailTemplate = _templateTypeRepository.GetEmailTemplate(emailTemplateId).Value;

                var emailPreset = emailTemplate.GetEmailPreset(emailPresetId);

                emailTemplate.RemovePreset(emailPreset);

                unitOfWork.SaveChanges();
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(Delete),
                "Email preset {EmailPresetId} deleted successfully",
                emailPresetId
            );
    }
}
