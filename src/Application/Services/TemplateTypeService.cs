namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing template types.
/// </summary>
public interface ITemplateTypeService
{
    /// <summary>
    /// Gets all template types.
    /// </summary>
    /// <returns>A <see cref="Result"/> of type <see cref="ImmutableList"/> containing all template types.</returns>
    Result<ImmutableList<TemplateType>> GetTemplateTypes();

    /// <summary>
    /// Gets all email templates from a specified template type.
    /// </summary>
    /// <param name="templateTypeId">The template type ID.</param>
    /// <returns>A <see cref="Result{TValue}"/> of a <see cref="IEnumerable{T}"/> of <see cref="EmailTemplate"/>.</returns>
    Result<IEnumerable<EmailTemplate>> GetEmailTemplates(Guid templateTypeId);

    /// <summary>
    /// Creates a template type.
    /// </summary>
    /// <param name="templateTypeDto">The template type DTO containing the information to create a template type.</param>
    /// <returns>A <see cref="Result"/> containing the ID of the newly created email template.</returns>
    Result<Guid> Create(TemplateTypeDto templateTypeDto);

    // TODO: Add delete method.
}

/// <inheritdoc />
public class TemplateTypeService(IUnitOfWork unitOfWork) : ITemplateTypeService
{
    private readonly ITemplateTypeRepository _templateTypeRepository =
        unitOfWork.GetRepository<ITemplateTypeRepository>();

    /// <inheritdoc />
    public Result<ImmutableList<TemplateType>> GetTemplateTypes() =>
        _templateTypeRepository.GetAll().ToImmutableList();

    /// <inheritdoc />
    public Result<Guid> Create(TemplateTypeDto templateTypeDto) =>
        Result<Guid>
            .Try(() =>
            {
                var templateType = templateTypeDto.ToEntity();

                _templateTypeRepository.Create(templateType);

                unitOfWork.SaveChanges();

                return templateType.Id;
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(Create),
                "Template type with ID {TemplateTypeId} created successfully",
                result => [result.Value]
            );

    /// <inheritdoc />
    public Result<IEnumerable<EmailTemplate>> GetEmailTemplates(Guid templateTypeId) =>
        Result<IEnumerable<EmailTemplate>>.Try(
            () => _templateTypeRepository.GetById(templateTypeId).EmailTemplates
        );
}
