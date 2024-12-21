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
    /// <param name="emailTemplateId">The ID of the email template to update the body from.</param>
    /// <param name="emailBodyContentDto">The new email body content DTO.</param>
    /// <returns>A <see cref="Result"/> representing the result of the operation.</returns>
    Result UpdateContent(Guid emailTemplateId, CreateEmailBodyContentDto emailBodyContentDto);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template to delete.</param>
    /// <returns>A <see cref="Result"/> representing the result of the operation.</returns>
    Result Delete(Guid emailTemplateId);
}

/// <inheritdoc />
public class EmailTemplateService(IUnitOfWork unitOfWork) : IEmailTemplateService
{
    private readonly IDataOwnerRepository _dataOwnerRepository =
        unitOfWork.GetRepository<IDataOwnerRepository>();

    private readonly ITemplateTypeRepository _templateTypeRepository =
        unitOfWork.GetRepository<ITemplateTypeRepository>();

    /// <inheritdoc />
    public Result<ReadEmailTemplateDto> Get(Guid id) =>
        _templateTypeRepository
            .GetEmailTemplate(id)
            .Match(emailTemplate => Result<ReadEmailTemplateDto>.Ok(emailTemplate.ToDto()));

    /// <inheritdoc />
    public Result<Guid> Create(CreateEmailTemplateDto createEmailTemplateDto) =>
        _templateTypeRepository
            .GetById(createEmailTemplateDto.TemplateTypeId)
            .Match(templateType => CreateEmailTemplate(templateType, createEmailTemplateDto));

    /// <inheritdoc />
    public Result UpdateContent(
        Guid emailTemplateId,
        CreateEmailBodyContentDto emailBodyContentDto
    ) =>
        _templateTypeRepository
            .GetEmailTemplate(emailTemplateId)
            .Match(emailTemplate => UpdateEmailBodyContent(emailTemplate, emailBodyContentDto));

    /// <inheritdoc />
    public Result Delete(Guid emailTemplateId) =>
        _templateTypeRepository
            .GetEmailTemplate(emailTemplateId)
            .Match(emailTemplate =>
            {
                // delegating this to a domain event would cause more headaches than benefits
                // data owner repo should probably not be used in this context (different aggregate)

                var fallBackEmailTemplateResult =
                    emailTemplate.TemplateType.GetFallBackEmailTemplate(emailTemplateId);

                return _dataOwnerRepository
                    .FallBackOrDeleteEmailSettings(emailTemplate.Id, fallBackEmailTemplateResult)
                    .Match(() =>
                    {
                        _templateTypeRepository.DeleteEmailTemplate(emailTemplate);
                        unitOfWork.SaveChanges();
                        return Result.Ok();
                    });
            });

    private Result<Guid> CreateEmailTemplate(
        TemplateType templateType,
        CreateEmailTemplateDto createEmailTemplateDto
    ) =>
        Result<EmailTemplate>
            .Try(() =>
            {
                var emailBodyContent = createEmailTemplateDto.EmailBodyContentDto.ToEntity(
                    templateType.AcceptedMergeTags
                );

                return createEmailTemplateDto.ToEntity(emailBodyContent);
            })
            .Match(emailTemplate =>
            {
                templateType.AddEmailTemplate(emailTemplate);

                unitOfWork.SaveChanges();

                return Result<Guid>.Ok(emailTemplate.Id);
            })
            .LogIf(
                true,
                "Email template with ID {Value} created successfully",
                result => [result.Value]
            );

    private Result UpdateEmailBodyContent(
        EmailTemplate emailTemplate,
        CreateEmailBodyContentDto emailBodyContentDto
    ) =>
        Result<EmailBodyContent>
            .Try(() => emailBodyContentDto.ToEntity(emailTemplate.TemplateType.AcceptedMergeTags))
            .Match(emailBodyContent =>
            {
                emailTemplate.UpdateContent(emailBodyContent);

                unitOfWork.SaveChanges();

                return Result.Ok();
            })
            .LogIf(true, "Email template with ID {Value} updated successfully", [emailTemplate.Id]);
}
