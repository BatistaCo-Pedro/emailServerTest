namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// A thin wrapper around a string that ensures it is valid HTML.
/// </summary>
[Serializable]
[JsonConverter(typeof(StringToWrapperJsonConverter<HtmlString>))]
public partial record HtmlString : NonEmptyString, IStringWrapper<HtmlString>
{
    // This can be configured to allow more or less elements and attributes depending on the requirements.
    private static readonly HtmlSanitizer Sanitizer;

    private static readonly SanitizationInfoBuilder SanitizationInfoBuilder;

    [GeneratedRegex("<[^>]+?>")]
    private static partial Regex MyRegex();
    
    /// <summary>
    /// Initializes static members of the <see cref="HtmlString"/> class and adds accepted values.
    /// </summary>
    static HtmlString()
    {
        SanitizationInfoBuilder = new SanitizationInfoBuilder();

        Sanitizer = new HtmlSanitizer();
        Sanitizer.AllowedAttributes.Add("class");
        Sanitizer.AllowedSchemes.Add("mailto");
        Sanitizer.AllowedSchemes.Add("tel");

        // listen to removing events to log them
        Sanitizer.RemovingAttribute += SanitizationInfoBuilder.AddRemovedAttribute;
        Sanitizer.RemovingStyle += SanitizationInfoBuilder.AddRemovedStyle;
        Sanitizer.RemovingTag += SanitizationInfoBuilder.AddRemovedTag;
        Sanitizer.RemovingCssClass += SanitizationInfoBuilder.AddRemovedCssClass;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlString"/> class.
    /// </summary>
    /// <param name="value">The non-empty string value to wrap.</param>
    public HtmlString(string value)
        : base(Sanitizer.Sanitize(value))
    {
        var sanitizationInfo = SanitizationInfoBuilder.Build();

        if (sanitizationInfo.RemovedElements.Length == 0)
        {
            return;
        }

        Log.Information(sanitizationInfo.ToString());
    }

    /// <summary>
    /// Strips all HTML tags from the string.
    /// </summary>
    /// <returns>A <see cref="NonEmptyString"/> without any HTML.</returns>
    public NonEmptyString StripHtml() => new(MyRegex().Replace(Value, string.Empty));

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
