namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// Interface for string wrappers.
/// </summary>
/// <typeparam name="T">The type of the wrapper.</typeparam>
/// <remarks>
/// Used for <see cref="StringToWrapperJsonConverter{T}"/>.
/// </remarks>
public interface IStringWrapper<out T>
{
    /// <summary>
    /// The wrapped string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Factory method to create a new instance of the wrapper from a string.
    /// </summary>
    /// <param name="value">The string value to create the wrapper from.</param>
    /// <returns>The wrapper of type <see cref="T"/>.</returns>
    /// <remarks>
    /// Important for the <see cref="StringToWrapperJsonConverter{TWrapper}"/>.
    /// </remarks>
    static abstract T Create(string value);
}
