namespace App.Server.Notification.Application.Domain.Common;

/// <summary>
/// Provides functions to validate variables.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Checks whether the value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void NotNull<T>([NotNull] T? value)
    {
        NotNull(value, $"Value for {typeof(T).Name} cannot be `null`.", typeof(T).Name);
    }

    /// <summary>
    /// Checks whether the value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void NotNull<T>([NotNull] T? value, string paramName)
    {
        NotNull(value, $"Value for {paramName} cannot be `null`.", paramName);
    }

    /// <summary>
    /// Checks whether the value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message for the exception.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void NotNull<T>([NotNull] T? value, string message, string paramName)
    {
        if (value == null)
        {
            throw new ArgumentException(message, paramName);
        }
    }

    /// <summary>
    /// Checks whether the value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void Null<T>(T? value)
    {
        Null(value, $"Value for {typeof(T).Name} must be `null`.", typeof(T).Name);
    }

    /// <summary>
    /// Checks whether the value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void Null<T>(T? value, string paramName)
    {
        Null(value, $"Value for {paramName} must be `null`.", paramName);
    }

    /// <summary>
    /// Checks whether the value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message for the exception.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void Null<T>(T? value, string message, string paramName)
    {
        if (value != null)
        {
            throw new ArgumentException(message, paramName);
        }
    }

    /// <summary>
    /// Checks whether the value is not null.
    /// </summary>
    /// <param name="values">The values to check.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void NotNull(IEnumerable<object?> values)
    {
        foreach (var value in values)
        {
            NotNull(value);
        }
    }

    /// <summary>
    /// Checks whether the value is not null.
    /// </summary>
    /// <param name="values">The values to check.</param>
    /// <param name="message">The message for the exception.</param>
    /// <exception cref="ArgumentException">If the value is null.</exception>
    public static void NotNull(IEnumerable<object?> values, string message)
    {
        foreach (var value in values)
        {
            NotNull(value, message, value?.GetType().Name ?? string.Empty);
        }
    }

    /// <summary>
    /// Checks whether an <see cref="IEnumerable{T}"/> is null or empty.
    /// </summary>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <exception cref="ArgumentNullException">If the enumerable is null or empty.</exception>
    public static void NotNullOrEmpty<T>([NotNull] IEnumerable<T>? enumerable)
    {
        NotNullOrEmpty(enumerable, "Enumerable cannot be null or empty.", typeof(T).Name);
    }

    /// <summary>
    /// Checks whether an <see cref="IEnumerable{T}"/> is null or empty.
    /// </summary>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <exception cref="ArgumentNullException">If the enumerable is null or empty.</exception>
    public static void NotNullOrEmpty<T>([NotNull] IEnumerable<T>? enumerable, string paramName)
    {
        NotNullOrEmpty(enumerable, "Enumerable cannot be null or empty.", paramName);
    }

    /// <summary>
    /// Checks whether an <see cref="IEnumerable{T}"/> is null or empty.
    /// </summary>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <param name="message">The message for the exception.</param>
    /// <param name="paramName">The name of the value.</param>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <exception cref="ArgumentNullException">If the enumerable is null or empty.</exception>
    public static void NotNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? enumerable,
        string message,
        string paramName
    )
    {
        if (enumerable == null || !enumerable.Any())
        {
            throw new ArgumentException(message, paramName);
        }
    }
}
