namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Helper class for string related operations.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Parses a string to one of the supported types passed in.
    /// Defaults to the string value if it couldn't be parsed to any other type.
    /// </summary>
    /// <param name="stringValue">The value as string.</param>
    /// <param name="supportedTypes">A list of supported types to try to parse to.</param>
    /// <param name="cultureInfo">The culture info to use for formatting.</param>
    /// <returns>The object successfully parsed - will always return the string value as a last resort.</returns>
    /// <remarks>
    /// Support <see cref="DateOnly"/>, <see cref="TimeOnly"/> and <see cref="Guid"/> besides the default conversion types.
    /// See <see cref="Convert.ChangeType(object?, Type)"/> for more information about the default types.
    /// Object, arrays and null are not supported.
    /// </remarks>
    public static object Parse(
        string stringValue,
        ImmutableArray<Type> supportedTypes,
        CultureInfo? cultureInfo = null
    )
    {
        var culture = cultureInfo ?? CultureInfo.InvariantCulture;

        foreach (var supportedType in supportedTypes)
        {
            if (TryParse(stringValue, supportedType, culture, out var result))
            {
                return result;
            }
        }

        // If no type could be parsed, default back to the string value.
        return stringValue;
    }

    /// <summary>
    /// Tries to parse a string value to a specific type.
    /// </summary>
    /// <param name="stringValue">The string value to parse.</param>
    /// <param name="type">The type to parse to.</param>
    /// <param name="cultureInfo">The culture info to use for formatting.</param>
    /// <param name="result">The parsing result, null if it couldn't be parsed.</param>
    /// <returns>A bool defining if the result was successfully parsed.</returns>
    /// <remarks>
    /// Support <see cref="DateOnly"/>, <see cref="TimeOnly"/> and <see cref="Guid"/> besides the default conversion types.
    /// See <see cref="Convert.ChangeType(object?, Type)"/> for more information about the default types.
    /// Object, arrays and null are not supported.
    /// </remarks>
    public static bool TryParse(
        string stringValue,
        Type type,
        CultureInfo? cultureInfo,
        [NotNullWhen(true)] out object? result
    )
    {
        var culture = cultureInfo ?? CultureInfo.InvariantCulture;

        try
        {
            result = Parse(stringValue, type, culture);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Couldn't parse value '{Value}' to type '{Type}'.", stringValue, type);
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Parses a string value to a specific type.
    /// </summary>
    /// <param name="stringValue">The string value to parse.</param>
    /// <param name="type">The type to parse to.</param>
    /// <param name="cultureInfo">The culture info to use for formatting.</param>
    /// <returns>The parsed type as an <see cref="object"/> - will always return the string value as last resort.</returns>
    /// <exception cref="InvalidCastException">Couldn't be cast to <paramref name="type"/>.</exception>
    /// <remarks>
    /// Support <see cref="DateOnly"/>, <see cref="TimeOnly"/> and <see cref="Guid"/> besides the default conversion types.
    /// See <see cref="Convert.ChangeType(object?, Type)"/> for more information about the default types.
    /// Object, arrays and null are not supported.
    /// </remarks>
    public static object Parse(
        NonEmptyString stringValue,
        Type type,
        CultureInfo? cultureInfo = null
    )
    {
        var culture = cultureInfo ?? CultureInfo.InvariantCulture;

        var parsedValue = SpecialParseableTypes
            .FirstOrDefault(x => x == type)
            ?.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, Params)!
            .Invoke(null, [(string)stringValue, culture]);

        return parsedValue ?? Convert.ChangeType(stringValue.Value, type, culture);
    }

    /// <summary>
    /// Param list of the parse method to get the parse method via reflection.
    /// </summary>
    private static readonly Type[] Params = [typeof(string), typeof(IFormatProvider)];

    /// <summary>
    /// Special parseable types not included in <see cref="Convert.ChangeType(object?, Type, IFormatProvider?)"/> but supported by us.
    /// </summary>
    private static readonly Type[] SpecialParseableTypes =
    [
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(Guid),
    ];
}
