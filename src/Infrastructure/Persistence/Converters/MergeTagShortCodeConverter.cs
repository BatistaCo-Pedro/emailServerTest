namespace App.Server.Notification.Infrastructure.Persistence.Converters;

/// <summary>
/// Converts <see cref="CultureCode"/> to <see cref="string"/> and vice versa for the database.
/// </summary>
internal sealed class MergeTagShortCodeConverter : ValueConverter<MergeTagShortCode, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CultureCodeConverter"/> class.
    /// </summary>
    public MergeTagShortCodeConverter()
        : base(
            nonEmptyString => (string)nonEmptyString,
            stringValue => new MergeTagShortCode(stringValue)
        ) { }
}
