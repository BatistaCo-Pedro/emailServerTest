namespace App.Server.Notification.Application.Domain.DataModels.Dtos;

/// <summary>
/// DTO for email preset.
/// </summary>
/// <param name="CustomMergeTags">The custom merge tags of the preset - must match the data required by the template.</param>
/// <param name="DataOwnerId">The ID of the template type the email template belongs to.</param>
/// <param name="EmailTemplateId">The ID of the email template this preset belongs to.</param>
/// <param name="DataOwnerId">The ID of the data owner this preset belongs to.</param>
[Serializable]
[method: JsonConstructor]
public record EmailPresetDto(
    ImmutableHashSet<CustomMergeTag> CustomMergeTags,
    Guid EmailTemplateId,
    Guid DataOwnerId
) : IDto, IEventMessage;
