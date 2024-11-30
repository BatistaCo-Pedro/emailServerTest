namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// Thin wrapper around a string that ensures it is not null, whitespace or empty.
/// </summary>
[Serializable]
[JsonConverter(typeof(StringToWrapperJsonConverter<NonEmptyString>))]
public record NonEmptyString : IStringWrapper<NonEmptyString>
{
    /// <inheritdoc />
    public string Value { get; }

    /// <summary>
    /// Ctor. Creates a new non-empty string.
    /// </summary>
    /// <param name="value">The string value to wrap.</param>
    /// <exception cref="ArgumentException">The string is null, whitespace or empty.</exception>
    public NonEmptyString(string value)
    {
        Value = !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException("Value cannot be null, whitespace or empty.");
    }

    /// <summary>
    /// Implicitly converts a non-empty string to a string.
    /// </summary>
    /// <param name="nonEmptyString">The non-empty string wrapper to unwrap.</param>
    /// <returns>The unwrapped non-empty string.</returns>
    public static implicit operator string(NonEmptyString nonEmptyString) => nonEmptyString.Value;

    /// <summary>
    /// Implicitly converts a string to a non-empty string.
    /// </summary>
    /// <param name="value">The string value to wrap.</param>
    /// <returns>A wrapped non-empty string value.</returns>
    public static implicit operator NonEmptyString(string value) => new(value);

    /// <inheritdoc />
    public static NonEmptyString Create(string value) => new(value);
}
