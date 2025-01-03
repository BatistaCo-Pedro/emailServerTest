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
    /// Gets the resource and the rest of the data in separate dictionaries.
    /// </summary>
    /// <param name="mergeTags">The merge tags from the <see cref="EmailBodyContent"/>.</param>
    /// <param name="mergeTagParameters"></param>
    /// <param name="customData"></param>
    /// <returns>A <see cref="Tuple{T1, T2}"/> of <see cref="ImmutableDictionary{TKey,TValue}"/> with the resources and data.</returns>
    public static (
        ImmutableDictionary<string, Resource> resources,
        ImmutableDictionary<string, string> data
    ) GetResourcesAndOthersSeparate(
        ImmutableHashSet<MergeTag> mergeTags,
        ImmutableDictionary<string, string> mergeTagParameters,
        ImmutableHashSet<CustomMergeTag> customData
    )
    {
        var customDataDictionary = customData.ToDictionary(
            x => (string)x.Name,
            x => (string)x.StringValue
        );
        var allData = mergeTagParameters.Concat(customDataDictionary).ToDictionary();

        var imageMergeTagNames = mergeTags
            .Where(x => x.Type == typeof(Resource))
            .Select(x => x.Name)
            .ToHashSet();

        var lookupTableByIsResource = allData.ToLookup(x => imageMergeTagNames.Contains(x.Key));

        var images = lookupTableByIsResource[true]
            .ToImmutableDictionary(x => x.Key, x => Resource.Create(x.Value, x.Key));
        return (images, lookupTableByIsResource[false].ToImmutableDictionary());
    }
}
