namespace App.Server.Notification.Application.Domain.Types.Result;

public static class ResultHelper
{
    public static Result<T> ToResult<T>(this T? value, string error) =>
        value != null ? Result<T>.Ok(value) : Result<T>.Fail(error);
}
