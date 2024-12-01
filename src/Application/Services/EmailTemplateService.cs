namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing email templates.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Gets an email template by its ID.
    /// </summary>
    /// <param name="id">The ID of the email template to get.</param>
    /// <returns>A <see cref="Result"/> of type <see cref="ReadEmailTemplateDto"/> representing the email template.</returns>
    Result<ReadEmailTemplateDto> Get(Guid id);

    /// <summary>
    /// Creates an email template.
    /// </summary>
    /// <param name="createEmailTemplateDto">The email template DTO containing the information to create the email template.</param>
    /// <returns>A <see cref="Result"/> containing the ID of the newly created email template.</returns>
    Result<Guid> Create(CreateEmailTemplateDto createEmailTemplateDto);

    /// <summary>
    /// Updates the body of an email template.
    /// </summary>
    /// <returns></returns>
    Result UpdateBody(Guid emailTemplateId, EmailBodyContentDto emailBodyContentDto);

    /// <summary>
    /// Deletes an email template and its presets.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template to delete.</param>
    /// <returns>A <see cref="Result"/> defining the success of the operation.</returns>
    Result Delete(Guid emailTemplateId);
}

/// <inheritdoc />
public class EmailTemplateService(IUnitOfWork unitOfWork) : IEmailTemplateService
{
    private readonly ITemplateTypeRepository _templateTypeRepository =
        unitOfWork.GetRepository<ITemplateTypeRepository>();

    /// <inheritdoc />
    public Result<ReadEmailTemplateDto> Get(Guid id) =>
        _templateTypeRepository.GetEmailTemplate(id).ToDto();

    /// <inheritdoc />
    public Result UpdateBody(Guid emailTemplateId, EmailBodyContentDto emailBodyContentDto) =>
        Result
            .Try(() =>
            {
                var emailTemplate = _templateTypeRepository.GetEmailTemplate(emailTemplateId);

                var emailBodyContent = emailBodyContentDto.ToEntity(
                    emailTemplate.TemplateType.AcceptedMergeTags
                );

                emailTemplate.UpdateContent(emailBodyContent);

                unitOfWork.SaveChanges();
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(UpdateBody),
                "Email template with ID {EmailTemplateId} updated successfully",
                emailTemplateId
            );

    /// <inheritdoc />
    public Result<Guid> Create(CreateEmailTemplateDto createEmailTemplateDto) =>
        Result<Guid>
            .Try(() =>
            {
                var templateType = _templateTypeRepository.GetById(
                    createEmailTemplateDto.TemplateTypeId
                );

                var emailBodyContent = createEmailTemplateDto.EmailBodyContentDto.ToEntity(
                    templateType.AcceptedMergeTags
                );

                var emailTemplateEntity = createEmailTemplateDto.ToEntity(emailBodyContent);

                templateType.AddEmailTemplate(emailTemplateEntity);

                unitOfWork.SaveChanges();

                return emailTemplateEntity.Id;
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(Create),
                "Email template with ID {EmailTemplateId} created successfully",
                result => [result.Value]
            );

    /// <inheritdoc />
    public Result Delete(Guid emailTemplateId) =>
        Result
            .Try(() =>
            {
                var emailTemplate = _templateTypeRepository.GetById(emailTemplateId);

                _templateTypeRepository.Delete(emailTemplate);

                unitOfWork.SaveChanges();
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(Delete),
                "Email template {EmailTemplateId} and its presets deleted successfully",
                emailTemplateId
            );
}
