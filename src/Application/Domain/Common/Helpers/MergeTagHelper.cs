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
}
