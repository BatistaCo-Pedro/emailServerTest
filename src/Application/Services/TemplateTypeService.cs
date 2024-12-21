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
    ImmutableHashSet<TemplateTypeDto> GetAll();

    /// <summary>
    /// Gets all email templates from a specified template type.
    /// </summary>
    /// <param name="templateTypeId">The template type ID.</param>
    /// <returns>A <see cref="Result{TValue}"/> of a <see cref="IEnumerable{T}"/> of <see cref="EmailTemplate"/>.</returns>
    Result<List<ReadEmailTemplateDto>> GetEmailTemplates(Guid templateTypeId);

    /// <summary>
    /// Creates a template type.
    /// </summary>
    /// <param name="templateTypeDto">The template type DTO containing the information to create a template type.</param>
    /// <returns>A <see cref="Result"/> containing the ID of the newly created template type.</returns>
    Result<Guid> Create(TemplateTypeDto templateTypeDto);
}

/// <inheritdoc />
public class TemplateTypeService(IUnitOfWork unitOfWork) : ITemplateTypeService
{
    private readonly ITemplateTypeRepository _templateTypeRepository =
        unitOfWork.GetRepository<ITemplateTypeRepository>();

    /// <inheritdoc />
    public ImmutableHashSet<TemplateTypeDto> GetAll() =>
        _templateTypeRepository.GetAll().Select(x => x.ToDto()).ToImmutableHashSet();

    /// <inheritdoc />
    public Result<List<ReadEmailTemplateDto>> GetEmailTemplates(Guid templateTypeId) =>
        _templateTypeRepository
            .GetById(templateTypeId)
            .Match(templateType =>
                Result<List<ReadEmailTemplateDto>>.Ok(
                    templateType.EmailTemplates.Select(x => x.ToDto()).ToList()
                )
            );

    /// <inheritdoc />
    public Result<Guid> Create(TemplateTypeDto templateTypeDto) =>
        Result<TemplateType>
            .Try(templateTypeDto.ToEntity)
            .Match(templateType =>
            {
                _templateTypeRepository.Create(templateType);

                unitOfWork.SaveChanges();

                return Result<Guid>.Ok(templateType.Id);
            })
            .LogIf(
                true,
                "Template type with ID {TemplateTypeId} created successfully",
                result => [result.Value]
            );
}
