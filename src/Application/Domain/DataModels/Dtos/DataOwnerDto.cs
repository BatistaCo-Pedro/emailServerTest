namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for owning entity.
/// </summary>
/// <param name="Name">The name of the entity.</param>
/// <param name="Source">The source of the entity.</param>
[Serializable]
[method: JsonConstructor]
public record DataOwnerDto(NonEmptyString Name, NonEmptyString Source) : IDto, IEventMessage;
