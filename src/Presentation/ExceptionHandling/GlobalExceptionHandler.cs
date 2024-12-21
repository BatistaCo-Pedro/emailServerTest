namespace App.Server.Notification.Presentation.ExceptionHandling;

/// <summary>
/// Handles exceptions globally.
/// </summary>
/// <remarks>
/// This class is registered in the DI container and is used to handle exceptions globally.
/// It will catch ACTUAL exceptions, not business logic errors.
/// Exceptions aren't used for flow control - see <see cref="Result{TValue}"/>.
/// </remarks>
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        Log.Error(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
