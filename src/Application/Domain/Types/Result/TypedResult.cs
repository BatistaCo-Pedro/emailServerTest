namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Default implementation of <see cref="IResult{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value in the result.</typeparam>
/// <remarks>
/// Structs don't return null, instead they return their default value.
/// There might be a need to handle classes and structs differently.
/// </remarks>
public sealed partial class Result<TValue> : IActionableResult<TValue, Result<TValue>>
{
    /// <summary>
    /// Pre allocated instance of <see cref="Result{TValue}"/> representing a failed result.
    /// </summary>
    private static readonly Result<TValue> FailedResult = new(Error.Empty);

    /// <inheritdoc />
    public ImmutableList<IError> Errors { get; }

    /// <summary>
    /// The value of the result or null.
    /// </summary>
    private readonly TValue? _valueOrDefault;

    /// <inheritdoc />
    public bool IsSuccess => Errors.Count == 0;

    /// <inheritdoc />
    public TValue Value
    {
        get
        {
            ThrowIfFailed();
            return _valueOrDefault!;
        }
        private init => _valueOrDefault = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    private Result()
    {
        Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the<see cref="Result{TValue}"/> class with the specified error.
    /// </summary>
    /// <param name="error">The error to initialize with.</param>
    private Result(IError error)
    {
        Errors = [error];
    }

    /// <summary>
    /// Initializes a new instance of the<see cref="Result{TValue}"/> class with the specified errors.
    /// </summary>
    /// <param name="errors">The errors to initialize with.</param>
    private Result(IEnumerable<IError> errors)
    {
        // ReSharper disable once UseCollectionExpression
        Errors = errors.ToImmutableList();
    }

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a success result with the specified value.</returns>
    public static Result<TValue> Ok(TValue value)
    {
        return new Result<TValue> { Value = value };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result.</returns>
    public static Result<TValue> Fail() => FailedResult;

    /// <summary>Creates a failed result with the given error message.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error message.</returns>
    public static Result<TValue> Fail(string errorMessage) => Fail(new Error(errorMessage));

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error message.</returns>
    public static Result<TValue> Fail(string errorMessage, (string Key, object Value) metadata)
    {
        var error = new Error(errorMessage, metadata);
        return Fail(error);
    }

    /// <summary>Creates a failed result with the given error message and metadata.</summary>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="metadata">The metadata associated with the failure.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error message.</returns>
    public static Result<TValue> Fail(string errorMessage, IDictionary<string, object> metadata)
    {
        var error = new Error(errorMessage, metadata);
        return Fail(error);
    }

    /// <summary>Creates a failed result with the given error.</summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error.</returns>
    public static Result<TValue> Fail(IError error)
    {
        return new Result<TValue>(error);
    }

    /// <summary>Creates a failed result with the given errors.</summary>
    /// <param name="errors">A collection of errors associated with the failure.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified errors.</returns>
    public static Result<TValue> Fail(IEnumerable<IError> errors)
    {
        return new Result<TValue>(errors);
    }

    /// <inheritdoc />
    public bool HasError<TError>()
        where TError : IError => Errors.Any(e => e is TError);

    /// <inheritdoc />
    public bool HasError(Type errorType) => Errors.Any(e => e.GetType() == errorType);

    /// <inheritdoc />
    public void ThrowIfFailed()
    {
        if (!IsSuccess)
            throw new ResultFailedException(this);
    }

    /// <summary>
    /// Implicitly convert an error to a failed result.
    /// </summary>
    /// <param name="value">The value to convert and include in the result.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> with the value.</returns>
    public static implicit operator Result<TValue>(TValue value)
    {
        if (value is Result<TValue> r)
            return r;

        return Ok(value);
    }

    /// <summary>
    /// Implicitly convert an error to a failed result.
    /// </summary>
    /// <param name="error">The error to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the error.</returns>
    public static implicit operator Result<TValue>(Error error)
    {
        return Fail(error);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(List<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(HashSet<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(ImmutableList<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a result to its value.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The value of the result.</returns>
    public static implicit operator TValue(Result<TValue> result)
    {
        return result.Value;
    }

    /// <summary>
    /// Implicitly convert a result to its error list.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The error list of the result.</returns>
    public static implicit operator ImmutableList<IError>(Result<TValue> result)
    {
        return result.Errors;
    }

    /// <summary>
    /// Deconstruct Result.
    /// </summary>
    /// <param name="isSuccess">Bool defining if the result is successful.</param>
    /// <param name="value">The value of the result in case of success or the default of the value.</param>
    /// <param name="errors">The errors from the result - empty in case of success.</param>
    public void Deconstruct(out bool isSuccess, out TValue? value, out ImmutableList<IError> errors)
    {
        isSuccess = IsSuccess;
        value = _valueOrDefault;
        errors = Errors;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsSuccess)
        {
            var valueString = ResultStringHelper.GetResultValueString(_valueOrDefault);
            return ResultStringHelper.GetResultString(nameof(Result), "True", valueString);
        }

        if (Errors[0].Message.Length == 0)
            return $"{nameof(Result)} {{ IsSuccess = False }}";

        var errorString = ResultStringHelper.GetResultErrorString(Errors);
        return ResultStringHelper.GetResultString(nameof(Result), "False", errorString);
    }
}
