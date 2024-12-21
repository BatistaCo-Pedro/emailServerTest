namespace App.Server.Notification.Application.Domain.Common.Helpers;

/// <summary>
/// Helper class for <see cref="Result{T}"/>.
/// </summary>
/// TODO: Add <see cref="ToResult{T}(T?,string)"/> methods accepting <see cref="IError"/>
public static class ResultHelper
{
    /// <summary>
    /// Converts a nullable value to a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The nullable value to convert.</param>
    /// <param name="error">The error for the failed result in case the value is null.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A <see cref="Result{TValue}"/> representing the value or an error.</returns>
    public static Result<T> ToResult<T>(this T? value, string error)
        where T : class => value is not null ? Result<T>.Ok(value) : Result<T>.Fail(error);

    /// <summary>
    /// Converts a nullable value to a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The nullable value to convert.</param>
    /// <param name="error">The error for the failed result in case the value is null.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A <see cref="Result{TValue}"/> representing the value or an error.</returns>
    public static Result<T> ToResult<T>(this T? value, string error)
        where T : struct =>
        value == null || value.Equals(default(T))
            ? Result<T>.Fail(error)
            : Result<T>.Ok(value.Value);
}
