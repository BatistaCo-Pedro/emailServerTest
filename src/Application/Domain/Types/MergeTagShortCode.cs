// ReSharper disable MemberCanBePrivate.Global
namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// A short code for a merge tag.
/// </summary>
[JsonConverter(typeof(StringToWrapperJsonConverter<MergeTagShortCode>))]
public record MergeTagShortCode : NonEmptyString, IStringWrapper<MergeTagShortCode>
{
    /// <summary>
    /// The beginning marker for a merge tag.
    /// </summary>
    internal const string MergeTagBeginMarker = "{{";

    /// <summary>
    /// The end marker for a merge tag.
    /// </summary>
    internal const string MergeTagEndMarker = "}}";

    /// <summary>
    /// The allowed characters for a short code name - not including the markers.
    /// </summary>
    internal static readonly string AllowedCharacters =
        $"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789{WhiteSpaceReplacement}.";

    private const char WhiteSpace = ' ';

    private const char WhiteSpaceReplacement = '_';

    /// <summary>
    /// Initializes a new instance of the <see cref="shortCode"/> class.
    /// </summary>
    /// <param name="shortCode">The short code as string.</param>
    /// <exception cref="MergeTagShortCode">If the short code passed in does not fill the rules for a short code.</exception>
    public MergeTagShortCode(string shortCode)
        : base(shortCode)
    {
        if (BreaksAnyRule(shortCode))
        {
            throw new ArgumentException(
                $"""
                Short code {shortCode} does not fulfill all requirements.
                Short codes must start with '{MergeTagBeginMarker}' and end with '{MergeTagEndMarker}' 
                and only contain allowed characters.
                Allowed characters: {AllowedCharacters}
                """
            );
        }
    }

    /// <summary>
    /// Represents the short code without the markers.
    /// </summary>
    public NonEmptyString ShortCodeWithoutMarkers => GetNameWithinMarkers(Value);

    /// <inheritdoc/>
    public new static MergeTagShortCode Create(string value) => new(value);

    /// <summary>
    /// Generates a short code from a string value.
    /// </summary>
    /// <param name="value">The value to generate a Moustache short code.</param>
    /// <returns>A <see cref="MergeTagShortCode"/> generated from the value passed in.</returns>
    public static MergeTagShortCode Generate(NonEmptyString value)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(MergeTagBeginMarker);

        var correctedValue = value
            .Value.Trim() // Also prevents leading and trailing white space replacements.
            // Replace whitespaces with its defined replacement.
            .Replace(WhiteSpace, WhiteSpaceReplacement);

        stringBuilder.Append(correctedValue.ToLower(CultureInfo.InvariantCulture));
        stringBuilder.Append(MergeTagEndMarker);
        return new MergeTagShortCode(stringBuilder.ToString());
    }

    /// <summary>
    /// Checks whether a short code breaks any of the rules.
    /// </summary>
    private static readonly Predicate<string> BreaksAnyRule = shortCode =>
    {
        return shortCode.LastIndexOf(MergeTagBeginMarker, StringComparison.Ordinal) != 0 // begin marker is at the start
            || shortCode.IndexOf(MergeTagEndMarker, StringComparison.Ordinal)
                != shortCode.Length - MergeTagBeginMarker.Length
            || GetNameWithinMarkers(shortCode).Any(c => !AllowedCharacters.Contains(c)); // end marker is at the end
    };

    /// <summary>
    /// Gets the name within the markers.
    /// </summary>
    /// <param name="shortCode">The short code to get the name from.</param>
    /// <returns>A <see cref="NonEmptyString"/> with the name.</returns>
    private static string GetNameWithinMarkers(string shortCode)
    {
        var beginIndex = shortCode.IndexOf(MergeTagBeginMarker, StringComparison.Ordinal);
        var endIndex = shortCode.LastIndexOf(MergeTagEndMarker, StringComparison.Ordinal);

        return shortCode.Substring(
            beginIndex + MergeTagBeginMarker.Length,
            endIndex - beginIndex - MergeTagEndMarker.Length
        );
    }
}
