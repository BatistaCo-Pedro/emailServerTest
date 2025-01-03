namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Helper class for string related operations.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Tries to parse a string value to a specific type.
    /// </summary>
    /// <param name="stringValue">The string value to parse.</param>
    /// <param name="type">The type to parse to.</param>
    /// <param name="cultureInfo">The culture info to use for formatting.</param>
    /// <param name="value">The parsing result, null if it couldn't be parsed.</param>
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
        [NotNullWhen(true)] out object? value
    )
    {
        var culture = cultureInfo ?? CultureInfo.InvariantCulture;

        // The check done this way does not throw an exception.
        if (!TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string)))
        {
            value = null;
            return false;
        }

        var parameters = new[] { typeof(string), typeof(IFormatProvider), type.MakeByRefType() };
        var arguments = new object[] { stringValue, culture, null! };

        // done this way because some of the primitive implementation do an interface implementation of IParsable<T>
        var interfaceMap = type.GetInterfaceMap(typeof(IParsable<>).MakeGenericType(type));
        var tryParseMethod = interfaceMap.TargetMethods.FirstOrDefault(x =>
            parameters.SequenceEqual(x.GetParameters().Select(p => p.ParameterType))
        );

        var result = tryParseMethod?.Invoke(null, arguments);

        value = arguments[2];
        return result as bool? ?? false;
    }

    /// <summary>
    /// Parses a string to a specific type.
    /// </summary>
    /// <param name="stringValue">The string to parse.</param>
    /// <param name="cultureInfo">The culture info for the parsing.</param>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <returns>The parsed value as type <see cref="T"/>.</returns>
    /// <exception cref="ArgumentException">Value couldn't be parsed.</exception>
    public static T Parse<T>(string stringValue, CultureInfo? cultureInfo = null)
        where T : IParsable<T>
    {
        if (TryParse(stringValue, typeof(T), cultureInfo, out var result))
        {
            return (T)result;
        }

        throw new ArgumentException(
            $"Value '{stringValue}' couldn't be parsed to type {typeof(T).Name}."
        );
    }

    /// <summary>
    /// Parses a string to a specific type.
    /// </summary>
    /// <param name="stringValue">The string to parse.</param>
    /// <param name="cultureInfo">The culture info for the parsing.</param>
    /// <param name="type">The type to cast to.</param>
    /// <returns>The parsed value as type <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">Value couldn't be parsed.</exception>
    public static object Parse(string stringValue, Type type, CultureInfo? cultureInfo = null)
    {
        if (TryParse(stringValue, type, cultureInfo, out var result))
        {
            return result;
        }

        throw new ArgumentException(
            $"Value '{stringValue}' couldn't be parsed to type {type.Name}."
        );
    }
}
