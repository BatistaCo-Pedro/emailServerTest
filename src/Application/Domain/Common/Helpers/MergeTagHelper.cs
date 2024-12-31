using App.Server.Notification.Application.Domain.DataModels.Emailing;

namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Helper class to work with merge tags.
/// </summary>
public static partial class MergeTagHelper
{
    [GeneratedRegex(@"\{\{(?:(?!}}).)*\}\}", RegexOptions.Multiline)]
    private static partial Regex MoustacheVariableRegex();

    /// <summary>
    /// Gets the merge tags base on the email structure and the accepted merge tags.
    /// </summary>
    /// <param name="emailStructure">The email structure.</param>
    /// <param name="acceptedMergeTags">The accepted <see cref="MergeTag"/> list from the template type.</param>
    /// <returns>A <see cref="Result{TValue}"/> with the result of the operation.</returns>
    public static Result<ImmutableHashSet<MergeTag>> GetMergeTags(
        JsonDocument emailStructure,
        IEnumerable<MergeTag> acceptedMergeTags
    )
    {
        return Result<ImmutableHashSet<MergeTag>>.Try(() =>
        {
            var foundMergeTagNames = MoustacheVariableRegex()
                .Matches(emailStructure.RootElement.GetRawText())
                .Select(x => new MergeTagShortCode(x.Value).ShortCodeWithoutMarkers) // also validates short-code
                .ToImmutableHashSet();

            // return every element in acceptedMergeTags where the name is in foundMergeTagNames
            return acceptedMergeTags
                .Where(x => foundMergeTagNames.Any(y => y == x.Name))
                // record is reference type, so we need to create a new instance (ChangeTracker might be tracking the accepted merge tags passed in)
                // This can be removes as soon as https://github.com/dotnet/efcore/issues/31252 gets merged.
                .Select(x => x with { })
                .ToImmutableHashSet();
        });
    }

    /// <summary>
    /// Gets a lookup of merge tags by image type.
    /// </summary>
    /// <param name="mergeTags">The merge tags from the <see cref="EmailBodyContent"/>.</param>
    /// <param name="mergeTagParameters"></param>
    /// <param name="customData"></param>
    /// <returns></returns>
    public static ILookup<bool, (string name, object value)> GetLookupByImageType(
        ImmutableHashSet<MergeTag> mergeTags,
        ImmutableDictionary<string, object> mergeTagParameters,
        ImmutableList<CustomMergeTag> customData
    )
    {
        var customDataLookUp = customData.ToLookup(
            x => x.Type == typeof(ResourceDto),
            x => ((string)x.Name, x.Value)
        );

        var imageMergeTagNames = mergeTags
            .Where(x => x.Type == typeof(ResourceDto))
            .Select(x => x.Name)
            .ToHashSet();

        var mergeTagParametersLookup = mergeTagParameters.ToLookup(
            x => imageMergeTagNames.Contains(x.Key),
            x => (x.Key, x.Value)
        );

        return customDataLookUp
            .Concat(mergeTagParametersLookup)
            .SelectMany(lookup => lookup.Select(value => new { lookup.Key, value }))
            .ToLookup(x => x.Key, x => x.value);
    }
}
