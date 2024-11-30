namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// This wrapper around a string - ensures that the culture code is valid.
/// </summary>
[Serializable]
[JsonConverter(typeof(StringToWrapperJsonConverter<CultureCode>))]
public record CultureCode : NonEmptyString, IStringWrapper<CultureCode>
{
    /// <summary>
    /// A read-only culture info object for the culture code.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureCode"/> class.
    /// </summary>
    /// <param name="cultureCode">The culture code to wrap.</param>
    /// <exception cref="ArgumentException">The culture code is not support by .NET.</exception>
    public CultureCode(string cultureCode)
        : base(cultureCode)
    {
        Culture =
            CultureInfo.GetCultureInfo(cultureCode)
            ?? throw new ArgumentException(
                $"Culture code '{cultureCode}' is not valid.",
                nameof(cultureCode)
            );
    }

    /// <summary>
    /// Implicitly converts a culture code to a string.
    /// </summary>
    /// <param name="cultureCode">The culture code to convert.</param>
    /// <returns>The unwrapped culture code as a string.</returns>
    public static implicit operator string(CultureCode cultureCode) => cultureCode.Value;

    /// <summary>
    /// Implicitly converts a string to a culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code string.</param>
    /// <returns>A wrapped, valid culture code.</returns>
    public static implicit operator CultureCode(string cultureCode) => new(cultureCode);

    /// <inheritdoc />
    public new static CultureCode Create(string value) => new(value);
}
