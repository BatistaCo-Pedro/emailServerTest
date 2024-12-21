namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Default implementation of <see cref="IResult"/>.
/// </summary>
public sealed partial class Result : IActionableResult<Result>
{
    /// <summary>
    /// Pre allocated instance of <see cref="Result"/> representing a successful result.
    /// </summary>
    private static readonly Result OkResult = new();

    /// <summary>
    /// Pre allocated instance of <see cref="Result"/> representing a failed result.
    /// </summary>
    private static readonly Result FailedResult = new(Error.Empty);

    /// <inheritdoc />
    public ImmutableList<IError> Errors { get; }

    /// <inheritdoc />
    public bool IsSuccess => Errors.Count == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    private Result()
    {
        Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with the specified error.
    /// </summary>
    /// <param name="error">The error to initialize with.</param>
    private Result(IError error)
    {
        Errors = [error];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with the specified errors.
    /// </summary>
    /// <param name="errors">The errors to initialize with.</param>
    private Result(IEnumerable<IError> errors)
    {
        // ReSharper disable once UseCollectionExpression
        Errors = errors.ToImmutableList();
    }

    /// <inheritdoc />
    public static Result Ok() => OkResult;

    /// <inheritdoc />
    public static Result Fail() => FailedResult;

    /// <inheritdoc />
    public static Result Fail(string errorMessage) => Fail(new Error(errorMessage));

    /// <inheritdoc />
    public static Result Fail(string errorMessage, (string Key, object Value) metadata) =>
        Fail(new Error(errorMessage, metadata));

    /// <inheritdoc />
    public static Result Fail(string errorMessage, IDictionary<string, object> metadata) =>
        Fail(new Error(errorMessage, metadata));

    /// <inheritdoc />
    public static Result Fail(IEnumerable<IError> errors) => new(errors);

    /// <inheritdoc />
    public Result MergeWith(params Result[] results)
    {
        var allResults = new HashSet<Result> { this };
        allResults.UnionWith(results);

        return MergeResults(allResults.ToArray());
    }

    /// <inheritdoc />
    public T Match<T>(Func<T> onSuccess, Func<IEnumerable<IError>, T> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Errors);

    /// <inheritdoc />
    public Result Match(Func<Result> onSuccess) => IsSuccess ? onSuccess() : Fail(Errors);

    /// <inheritdoc />
    public IActionResult Match() =>
        IsSuccess
            ? new OkResult()
            : new BadRequestObjectResult(ResultStringHelper.GetResultErrorString(Errors));

    /// <inheritdoc />
    public static Result Fail(IError error) => new(error);

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
    /// <param name="error">The error to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result"/> with the error.</returns>
    public static implicit operator Result(Error? error)
    {
        return Fail(error ?? Error.Empty);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result"/> with the errors.</returns>
    public static implicit operator Result(List<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result"/> with the errors.</returns>
    public static implicit operator Result(HashSet<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result"/> with the errors.</returns>
    public static implicit operator Result(ImmutableList<IError> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a result to its error list.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The error list of the result.</returns>
    public static implicit operator ImmutableList<IError>(Result result)
    {
        return result.Errors;
    }

    /// <summary>
    /// Deconstruct Result.
    /// </summary>
    /// <param name="isSuccess">Bool defining if the result is successful.</param>
    /// <param name="errors">The errors from the result - empty in case of success.</param>
    public void Deconstruct(out bool isSuccess, out ImmutableList<IError> errors)
    {
        isSuccess = IsSuccess;
        errors = Errors;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsSuccess)
            return $"{nameof(Result)} {{ IsSuccess = True }}";

        if (Errors[0].Message.Length == 0)
            return $"{nameof(Result)} {{ IsSuccess = False }}";

        var errorString = ResultStringHelper.GetResultErrorString(Errors);
        return ResultStringHelper.GetResultString(nameof(Result), "False", errorString);
    }
}
