namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for template type.
/// </summary>
/// <param name="Name">The name of the template.</param>
/// <param name="AcceptedMergeTags">The list of accepted merge tags for the type.</param>
[Serializable]
[method: JsonConstructor]
public record TemplateTypeDto(NonEmptyString Name, ImmutableHashSet<MergeTag> AcceptedMergeTags)
    : IDto,
        IEventMessage;
