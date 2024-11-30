namespace App.Server.Notification.Tests.Unit.TestData;

/// <summary>
/// A test error class inheriting from <see cref="IError" />.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestError" />.
/// </remarks>
/// <param name="message">The error message.</param>
internal class TestError(string message) : IError
{
    /// <inheritdoc />
    public string Message { get; } = message;

    /// <inheritdoc />
    public FrozenDictionary<string, object> Metadata { get; } =
        FrozenDictionary<string, object>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestError" /> class.
    /// </summary>
    private TestError()
        : this(string.Empty) { }
}
