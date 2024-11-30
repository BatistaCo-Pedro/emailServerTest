namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Helper class for working with moustache variables.
/// </summary>
public static partial class MergeTagHelper
{
    [GeneratedRegex(@"\{\{(?:(?!}}).)*\}\}", RegexOptions.Multiline)]
    private static partial Regex MoustacheVariableRegex();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailStructure"></param>
    /// <param name="acceptedMergeTags"></param>
    /// <returns></returns>
    public static Result<ImmutableHashSet<MergeTag>> GetMergeTags(
        JsonDocument emailStructure,
        ImmutableHashSet<MergeTag> acceptedMergeTags
    )
    {
        return Result<ImmutableHashSet<MergeTag>>.Try(() =>
        {
            var mergeTagNames = MoustacheVariableRegex()
                .Matches(emailStructure.RootElement.GetRawText())
                .Select(x => new MergeTagShortCode(x.Value).ShortCodeWithoutMarkers) // also validates short-code
                .ToImmutableHashSet();

            // return every element in acceptedMergeTags where the name is in mergeTagNames
            return acceptedMergeTags
                .Where(x => mergeTagNames.Any(y => y == x.Name))
                .ToImmutableHashSet();
        });
    }
}
