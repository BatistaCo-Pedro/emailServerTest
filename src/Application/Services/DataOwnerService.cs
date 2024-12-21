namespace App.Server.Notification.Application.Services;

/// <summary>
/// Service for managing data owners.
/// </summary>
public interface IDataOwnerService
{
    /// <summary>
    /// Gets a data owner by its ID.
    /// </summary>
    /// <param name="dataOwnerId">The ID of the data owner to get.</param>
    /// <returns>A <see cref="Result"/> of type <see cref="DataOwnerDto"/> representing the data owner.</returns>
    Result<DataOwnerDto> Get(Guid dataOwnerId);

    /// <summary>
    /// Creates data owner
    /// </summary>
    /// <param name="dataOwnerDto">The DTO containing the information to create a data owner.</param>
    /// <returns>A <see cref="Result"/> with the ID of the created data owner.</returns>
    Result<Guid> Create(DataOwnerDto dataOwnerDto);

    /// <summary>
    /// Updates the data belonging to a data owner.
    /// </summary>
    /// <param name="dataOwnerDto">The data owner DTO.</param>
    /// <returns>A <see cref="Result"/> containing the ID of the newly created email preset.</returns>
    Result UpdateData(DataOwnerDto dataOwnerDto);

    /// <summary>
    ///  Updates the SMTP settings.
    /// </summary>
    /// <param name="smtpSettingsDto">The smtp settings DTO.</param>
    /// <returns>A <see cref="Result"/> object representing the result of the operation.</returns>
    Result UpdateSmtpSettings(SmtpSettingsDto smtpSettingsDto);

    /// <summary>
    /// Updates the default email template of a template type for a data owner.
    /// </summary>
    /// <param name="emailSettingsDto">The new email settings information.</param>
    /// <returns></returns>
    Result UpdateDefaultEmailTemplate(EmailSettingsDto emailSettingsDto);
}

/// <inheritdoc />
public class DataOwnerService(IUnitOfWork unitOfWork) : IDataOwnerService
{
    private readonly IDataOwnerRepository _dataOwnerRepository =
        unitOfWork.GetRepository<IDataOwnerRepository>();

    /// <inheritdoc />
    public Result<DataOwnerDto> Get(Guid dataOwnerId) =>
        _dataOwnerRepository
            .GetById(dataOwnerId)
            .Match(value => Result<DataOwnerDto>.Ok(value.ToDto()));

    /// <inheritdoc />
    public Result<Guid> Create(DataOwnerDto dataOwnerDto) =>
        Result<DataOwner>
            .Try(dataOwnerDto.ToEntity)
            .Match(dataOwner =>
            {
                _dataOwnerRepository.Create(dataOwner);

                unitOfWork.SaveChanges();
                return Result<Guid>.Ok(dataOwner.Id);
            })
            .LogIf(
                true,
                "Data Owner with ID {DataOwnerId} created successfully",
                result => [result.Value]
            );

    /// <inheritdoc />
    public Result UpdateData(DataOwnerDto dataOwnerDto) =>
        _dataOwnerRepository
            .GetByUniqueProperties(dataOwnerDto.Name, dataOwnerDto.Source)
            .Match(dataOwner => UpdateData(dataOwner, dataOwnerDto.Data));

    /// <inheritdoc />
    public Result UpdateSmtpSettings(SmtpSettingsDto smtpSettingsDto) =>
        _dataOwnerRepository
            .GetById(smtpSettingsDto.DataOwnerId)
            .Match(dataOwner =>
            {
                dataOwner.UpdateSmtpSettings(smtpSettingsDto.SmtpSettings);
                return Result.Ok();
            });

    /// <inheritdoc />
    public Result UpdateDefaultEmailTemplate(EmailSettingsDto emailSettingsDto) =>
        _dataOwnerRepository
            .GetById(emailSettingsDto.DataOwnerId)
            .Match(dataOwner =>
                dataOwner.GetEmailSettingsByTemplateTypeId(emailSettingsDto.TemplateTypeId)
            )
            .Match(emailSettings =>
            {
                emailSettings.ReplaceDefaultEmailTemplate(emailSettingsDto.DefaultEmailTemplateId);
                return Result.Ok();
            });

    private Result UpdateData(DataOwner dataOwner, ImmutableHashSet<CustomMergeTagDto> data) =>
        Result<ImmutableHashSet<CustomMergeTag>>
            .Try(() => data.Select(x => x.ToEntity()).ToImmutableHashSet())
            .Match(customMergeTags =>
            {
                dataOwner.UpdateCustomMergeTags(customMergeTags);

                unitOfWork.SaveChanges();
                return Result.Ok();
            })
            .LogIf(
                true,
                "Data for DataOwner with ID {DataOwnerId} updated successfully",
                [dataOwner.Id]
            );
}
