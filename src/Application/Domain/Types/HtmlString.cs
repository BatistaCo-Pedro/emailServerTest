namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// A thin wrapper around a string that ensures it is valid HTML.
/// </summary>
[Serializable]
[JsonConverter(typeof(StringToWrapperJsonConverter<HtmlString>))]
public record HtmlString : NonEmptyString, IStringWrapper<HtmlString>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlString"/> class.
    /// </summary>
    /// <param name="value">The non-empty string value to wrap.</param>
    public HtmlString(string value)
        : base(value)
    {
        // TODO: Validate that the string is valid HTML.
        // TODO: Sanitize the HTML string.
    }

    /// <summary>
    /// Implicitly converts an HTML string to a string.
    /// </summary>
    /// <param name="htmlString">The HTML string wrapper to unwrap.</param>
    /// <returns>The unwrapped string.</returns>
    public static implicit operator string(HtmlString htmlString) => htmlString.Value;

    /// <summary>
    /// Implicitly converts a non-empty string to an HTML string.
    /// </summary>
    /// <param name="value">The non-empty string value to wrap.</param>
    /// <returns>A wrapped HTML string value.</returns>
    public static implicit operator HtmlString(string value) => new(value);

    /// <inheritdoc />
    public new static HtmlString Create(string value) => new(value);
}
