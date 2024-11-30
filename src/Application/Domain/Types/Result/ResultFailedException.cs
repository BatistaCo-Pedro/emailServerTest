namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Exception thrown when a result is failed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultFailedException"/> class.
/// </remarks>
/// <param name="result">The result to get the errors from.</param>
public class ResultFailedException(IResult result) : Exception(result.ToString());
