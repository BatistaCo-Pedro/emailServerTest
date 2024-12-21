namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for custom merge tag.
/// </summary>
/// <param name="Name">The name of the custom merge tag.</param>
/// <param name="StringValue">The value of the custom merge tag as a string.</param>
/// TODO: Get type from the frontend. Finding the type via the string value may lead to issues
/// TODO: such as having "1" register as an int even though it might be wished to have it as a string.
[Serializable]
[method: JsonConstructor]
public record CustomMergeTagDto(NonEmptyString Name, NonEmptyString StringValue)
    : IDto,
        IEventMessage;
