namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Defines an error with a message and associated metadata.
/// </summary>
public interface IError
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the metadata associated with the error.
    /// </summary>
    /// <remarks>The metadata is represented as a dictionary of key-value pairs.</remarks>
    public FrozenDictionary<string, object> Metadata { get; }
}

/// <summary>
/// Default implementation of the <see cref="IError"/> interface.
/// </summary>
public class Error : IError
{
    /// <summary>
    /// Represents an empty error.
    /// </summary>
    public static Error Empty { get; } = new();

    /// <inheritdoc />
    public string Message { get; }

    /// <inheritdoc />
    public FrozenDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IError"/> class.
    /// </summary>
    private Error()
        : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public Error(string message)
    {
        Message = message;
        Metadata = FrozenDictionary<string, object>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message and metadata.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">The metadata associated with the error.</param>
    public Error(string message, (string Key, object Value) metadata)
    {
        Message = message;
        var dictionary = new Dictionary<string, object> { { metadata.Key, metadata.Value } };
        Metadata = dictionary.ToFrozenDictionary();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message and metadata.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">The metadata associated with the error.</param>
    public Error(string message, IDictionary<string, object> metadata)
    {
        Message = message;
        Metadata = metadata.ToFrozenDictionary();
    }

    /// <inheritdoc />
    public override string ToString() => ResultStringHelper.GetErrorString(this);
}
