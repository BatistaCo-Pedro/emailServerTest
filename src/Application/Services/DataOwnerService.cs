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
    Result<DataOwnerDto> GetDataOwner(Guid dataOwnerId);

    /// <summary>
    /// Creates data owner
    /// </summary>
    /// <param name="dataOwnerDto">The DTO containing the information to create a data owner.</param>
    /// <returns>A <see cref="Result"/> with the ID of the created data owner.</returns>
    Result<Guid> CreateDataOwner(DataOwnerDto dataOwnerDto);

    // TODO: Add delete method
}

/// <inheritdoc />
public class DataOwnerService(IUnitOfWork unitOfWork) : IDataOwnerService
{
    private readonly ICrudRepository<DataOwner> _dataOwnerRepository =
        unitOfWork.GetRepository<ICrudRepository<DataOwner>>();

    /// <inheritdoc />
    public Result<DataOwnerDto> GetDataOwner(Guid dataOwnerId)
    {
        return _dataOwnerRepository.GetById(dataOwnerId).ToDto();
    }

    /// <inheritdoc />
    public Result<Guid> CreateDataOwner(DataOwnerDto dataOwnerDto)
    {
        return Result<Guid>
            .Try(() =>
            {
                var dataOwner = dataOwnerDto.ToEntity();

                _dataOwnerRepository.Create(dataOwner);

                unitOfWork.SaveChanges();

                return dataOwner.Id;
            })
            .LogIfSuccess(
                LogEventLevel.Information,
                nameof(CreateDataOwner),
                "Data Owner with ID {DataOwnerId} created successfully",
                result => [result.Value]
            );
    }
}
