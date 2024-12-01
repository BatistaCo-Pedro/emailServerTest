#pragma warning disable RMG012 // Source member was not found for target member
#pragma warning disable RMG020 // Source member is not mapped to any target member

namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// Mapper class for mapping between domain entities and DTOs.
/// </summary>
[Mapper(PreferParameterlessConstructors = false)]
public static partial class Mapper
{
    /// <summary>
    /// Maps an <see cref="EmailTemplate"/> to an <see cref="ReadEmailTemplateDto"/>.
    /// </summary>
    /// <param name="emailTemplate">The entity to map.</param>
    /// <returns>A <see cref="ReadEmailTemplateDto"/> representing the <paramref name="emailTemplate"/>.</returns>
    [MapPropertyFromSource(
        nameof(ReadEmailTemplateDto.EmailJsonStructures),
        Use = nameof(GetEmailTemplateBodyStructures)
    )]
    public static partial ReadEmailTemplateDto ToDto(this EmailTemplate emailTemplate);

    /// <summary>
    /// Maps an <see cref="CreateEmailTemplateDto"/> to an <see cref="EmailTemplate"/>.
    /// </summary>
    /// <param name="createEmailTemplateDto">The dto to map.</param>
    /// <param name="emailBodyContent"></param>
    /// <returns>A <see cref="EmailTemplate"/> entity from the <paramref name="createEmailTemplateDto"/>.</returns>
    public static partial EmailTemplate ToEntity(
        this CreateEmailTemplateDto createEmailTemplateDto, EmailBodyContent emailBodyContent
    );

    /// <summary>
    /// Maps an <see cref="EmailPreset"/> to an <see cref="EmailPresetDto"/>.
    /// </summary>
    /// <param name="emailPreset">The entity to map.</param>
    /// <returns>A <see cref="EmailPresetDto"/> representing the <paramref name="emailPreset"/>.</returns>
    public static partial EmailPresetDto ToDto(this EmailPreset emailPreset);

    /// <summary>
    /// Maps an <see cref="EmailPresetDto"/> to an <see cref="EmailPreset"/>.
    /// </summary>
    /// <param name="emailPresetDto">The dto to map.</param>
    /// <returns>A <see cref="EmailPreset"/> entity from the <paramref name="emailPresetDto"/>.</returns>
    public static partial EmailPreset ToEntity(this EmailPresetDto emailPresetDto);

    /// <summary>
    /// Maps a <see cref="DataOwner"/> to a <see cref="DataOwnerDto"/>.
    /// </summary>
    /// <param name="dataOwner">The entity to map.</param>
    /// <returns>A <see cref="DataOwnerDto"/> representing the <paramref name="dataOwner"/>.</returns>
    public static partial DataOwnerDto ToDto(this DataOwner dataOwner);

    /// <summary>
    /// Maps a <see cref="DataOwnerDto"/> to a <see cref="DataOwner"/>.
    /// </summary>
    /// <param name="dataOwnerDto">The dto to map.</param>
    /// <returns>A <see cref="DataOwner"/> entity from the <paramref name="dataOwnerDto"/>.</returns>
    public static partial DataOwner ToEntity(this DataOwnerDto dataOwnerDto);

    /// <summary>
    /// Maps a <see cref="TemplateType"/> to a <see cref="TemplateTypeDto"/>.
    /// </summary>
    /// <param name="templateType">The entity to map.</param>
    /// <returns>A <see cref="TemplateTypeDto"/> representing the <paramref name="templateType"/>.</returns>
    public static partial TemplateTypeDto ToDto(this TemplateType templateType);

    /// <summary>
    /// Maps a <see cref="TemplateTypeDto"/> to a <see cref="TemplateType"/>.
    /// </summary>
    /// <param name="templateTypeDto">The dto to map.</param>
    /// <returns>A <see cref="TemplateType"/> entity from the <paramref name="templateTypeDto"/>.</returns>
    public static partial TemplateType ToEntity(this TemplateTypeDto templateTypeDto);

    /// <summary>
    /// Maps a <see cref="EmailBodyContent"/> to a <see cref="EmailBodyContentDto"/>.
    /// </summary>
    /// <param name="emailBodyContent">The entity to map.</param>
    /// <returns>A <see cref="EmailBodyContentDto"/> representing the <paramref name="emailBodyContent"/>.</returns>
    public static partial EmailBodyContentDto ToDto(this EmailBodyContent emailBodyContent);

    /// <summary>
    /// Maps a <see cref="EmailBodyContentDto"/> to a <see cref="EmailBodyContent"/>.
    /// </summary>
    /// <param name="emailBodyContentDto">The dto to map.</param>
    /// <param name="acceptedMergeTags">The <see cref="ImmutableHashSet{T}"/> of accepted merge tags for the type this content is being created for.</param>
    /// <returns>An <see cref="EmailBodyContent"/> entity from the <paramref name="emailBodyContentDto"/>.</returns>
    public static partial EmailBodyContent ToEntity(this EmailBodyContentDto emailBodyContentDto, IEnumerable<MergeTag> acceptedMergeTags);

    /// <summary>
    /// Maps a <see cref="CustomMergeTag"/> to a <see cref="CustomMergeTagDto"/>.
    /// </summary>
    /// <param name="customMergeTag">The entity to map.</param>
    /// <returns>A <see cref="CustomMergeTagDto"/> representing the <paramref name="customMergeTag"/>.</returns>
    public static partial CustomMergeTagDto ToDto(this CustomMergeTag customMergeTag);

    /// <summary>
    /// Maps a <see cref="CustomMergeTagDto"/> to a <see cref="CustomMergeTag"/>.
    /// </summary>
    /// <param name="customMergeTagDto">The dto to map.</param>
    /// <returns>An <see cref="CustomMergeTag"/> entity from the <paramref name="customMergeTagDto"/>.</returns>
    public static partial CustomMergeTag ToEntity(this CustomMergeTagDto customMergeTagDto);

    private static ImmutableList<JsonDocument> GetEmailTemplateBodyStructures(
        EmailTemplate emailTemplate
    ) => emailTemplate.EmailBodyContents.Select(x => x.JsonStructure).ToImmutableList();
}
