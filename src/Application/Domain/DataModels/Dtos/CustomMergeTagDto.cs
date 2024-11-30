namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for custom merge tag.
/// </summary>
/// <param name="Name">The name of the custom merge tag.</param>
/// <param name="StringValue">The value of the custom merge tag as a string.</param>
public record CustomMergeTagDto(NonEmptyString Name, NonEmptyString StringValue)
    : IDto,
        IEventMessage;
