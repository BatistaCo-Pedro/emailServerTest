namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Interface for a result.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the result was successful.
    /// </summary>
    /// <returns><c>true</c> if the result was successful; otherwise, <c>false</c>.</returns>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a collection of errors associated with the result.
    /// </summary>
    /// <returns>
    /// An <see cref="ImmutableArray{T}"/> of <see cref="IError"/> representing the errors.
    /// </returns>
    ImmutableList<IError> Errors { get; }

    /// <summary>
    /// Checks if the result contains an error of the specific type.
    /// </summary>
    /// <typeparam name="TError">The type of error to check for.</typeparam>
    /// <returns><c>true</c> if an error of the specified type is present, otherwise <c>false</c>.</returns>
    bool HasError<TError>()
        where TError : IError;

    /// <summary>
    /// Checks if the result contains an error of the specific type.
    /// </summary>
    /// <param name="errorType">The type of error to check for.</param>
    /// <returns><c>true</c> if an error of the specified type is present; otherwise, <c>false</c>.</returns>
    bool HasError(Type errorType);

    /// <summary>
    /// Throws an exception if the result is failed.
    /// </summary>
    /// <exception cref="ResultFailedException">Result is failed.</exception>
    void ThrowIfFailed();
}

/// <summary>
/// Interface for a typed result.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public interface IResult<out TValue> : IResult
{
    /// <summary>
    /// Gets the value of the result, throwing an exception if the result is failed.
    /// </summary>
    /// <returns>The value of the result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to get or set the value of a failed result.</exception>
    public TValue Value { get; }
}

/// <summary>
/// Defines an auditable result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IAuditableResult<out TResult>
    where TResult : IResult
{
    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="messageTemplate">The message to log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValues">The values to inject in the template.</param>
    /// <returns>The result object.</returns>
    TResult Log(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        params object?[] propertyValues
    );

    /// <summary>
    /// Logs the result if successful.
    /// </summary>
    /// <param name="messageTemplate">The message to log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValues">The values to inject in the template.</param>
    /// <returns>The result object.</returns>
    TResult LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        params object?[] propertyValues
    );

    /// <summary>
    /// Logs the result if successful.
    /// </summary>
    /// <param name="messageTemplate">The message to log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValues">The values to inject in the template.</param>
    /// <returns>The result object.</returns>
    TResult LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        params object?[] propertyValues
    );

    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValuesSelector">A selector for the values to be injected in the template.</param>
    /// <returns>The result object.</returns>
    TResult Log(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<TResult, object?[]> propertyValuesSelector
    );

    /// <summary>
    /// Logs the result if successful.
    /// </summary>
    /// <param name="messageTemplate">The message to template log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValuesSelector">A selector for the values to be injected in the template.</param>
    /// <returns>The result object.</returns>
    TResult LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<TResult, object?[]> propertyValuesSelector
    );

    /// <summary>
    /// Logs the result if successful.
    /// </summary>
    /// <param name="messageTemplate">The message to template log.</param>
    /// <param name="context">The context from where the log comes.</param>
    /// <param name="logLevel">The level which to log at.</param>
    /// <param name="propertyValuesSelector">A selector for the values to be injected in the template.</param>
    /// <returns>The result object.</returns>
    TResult LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<TResult, object?[]> propertyValuesSelector
    );
}

/// <summary>
/// Defines an actionable result.
/// </summary>
public interface IActionableResult<TResult> : IResult
    where TResult : IActionableResult<TResult>
{
    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a success result with the specified value.</returns>
    static abstract TResult Ok();

    /// <summary>Creates a failed result.</summary>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result.</returns>
    static abstract TResult Fail();

    /// <summary>Creates a failed result with the given error message.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message.</returns>
    static abstract TResult Fail(string errorMessage);

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message and metadata.</returns>
    static abstract TResult Fail(string errorMessage, (string Key, object Value) metadata);

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message and metadata.</returns>
    static abstract TResult Fail(string errorMessage, IDictionary<string, object> metadata);

    /// <summary>Creates a failed result with the given error.</summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error.</returns>
    static abstract TResult Fail(IError error);

    /// <summary>Creates a failed result with the given errors.</summary>
    /// <param name="errors">A collection of errors associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified errors.</returns>
    static abstract TResult Fail(IEnumerable<IError> errors);

    /// <summary>
    /// Wraps an action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result representing the result from the action.</returns>
    static abstract TResult Try(
        Action action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(Try)
    );

    /// <summary>
    /// Wraps an asynchronous action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result representing the result from the action.</returns>
    static abstract Task<TResult> TryAsync(
        Func<Task> action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(TryAsync)
    );

    /// <summary>
    /// Wraps an asynchronous action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result representing the result from the action.</returns>
    static abstract Task<TResult> TryAsync(
        Func<ValueTask> action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(TryAsync)
    );

    /// <summary>
    /// Merges all the results passed in. <br/>
    /// If none of the results is failed, returns a successful result. <br/>
    /// If any result is failed, merges all errors to one result.
    /// </summary>
    /// <param name="results">The <see cref="Result"/> objects to merge.</param>
    /// <returns>A successful <see cref="Result"/> or one containing all merged errors.</returns>
    static abstract TResult MergeResults(params TResult[] results);

    /// <summary>
    /// Merges the result with the results passed in.
    /// </summary>
    /// <param name="results">The results to merge with.</param>
    /// <returns>A successful <see cref="Result"/> or one containing all merged errors.</returns>
    TResult MergeWith(params TResult[] results);
}

/// <summary>Defines an actionable result.</summary>
public interface IActionableResult<TValue, TResult> : IResult<TValue>
    where TResult : IActionableResult<TValue, TResult>
{
    /// <summary>Creates a success result with the given value.</summary>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a success result with the specified value.</returns>
    static abstract TResult Ok(TValue value);

    /// <summary>Creates a failed result.</summary>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result.</returns>
    static abstract TResult Fail();

    /// <summary>Creates a failed result with the given error message.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message.</returns>
    static abstract TResult Fail(string errorMessage);

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message and metadata.</returns>
    static abstract TResult Fail(string errorMessage, (string Key, object Value) metadata);

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error message and metadata.</returns>
    static abstract TResult Fail(string errorMessage, IDictionary<string, object> metadata);

    /// <summary>Creates a failed result with the given error.</summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified error.</returns>
    static abstract TResult Fail(IError error);

    /// <summary>Creates a failed result with the given errors.</summary>
    /// <param name="errors">A collection of errors associated with the failure.</param>
    /// <returns>A new instance of <typeparamref name="TResult" /> representing a failed result with the specified errors.</returns>
    static abstract TResult Fail(IEnumerable<IError> errors);

    T Match<T>(Func<TValue, T> onSuccess, Func<IError, T> onError) where T : class;

    Result<T> Match<T>(Func<TValue, T> onSuccess) where T : class;
    
    /// <summary>
    /// Wraps an asynchronous action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result of type <see cref="TValue"/> representing the result from the action.</returns>
    static abstract TResult Try(
        Func<TValue> action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(Try)
    );

    /// <summary>
    /// Wraps an asynchronous action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result of type <see cref="TValue"/> representing the result from the action.</returns>
    static abstract Task<TResult> TryAsync(
        Func<Task<TValue>> action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(TryAsync)
    );

    /// <summary>
    /// Wraps an asynchronous action in a try-catch block and returns a result.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">A custom exception handler to handle the caught exceptions.</param>
    /// <param name="context">The context for logging.</param>
    /// <returns>A result of type <see cref="TValue"/> representing the result from the action.</returns>
    static abstract Task<TResult> TryAsync(
        Func<ValueTask<TValue>> action,
        Func<Exception, IError>? exceptionHandler = null,
        string context = nameof(TryAsync)
    );
}
