namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Partial class of <see cref="Result"/> for extensions.
/// </summary>
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
public sealed partial class Result : IAuditableResult<Result>
{
    /// <inheritdoc />
    public Result Log(
        [ConstantExpected] string messageTemplate = "",
        object?[]? propertyValues = null,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(Log)
    )
    {
        var template = "Context: {Context} - Result: {Result} - Message: " + messageTemplate;
        var allPropertyValues = new object?[(propertyValues?.Length ?? 0) + 2];
        allPropertyValues[0] = context;
        allPropertyValues[1] = ToString();
        propertyValues?.CopyTo(allPropertyValues, 2);

        Serilog.Log.Write(logLevel, template, allPropertyValues);
        return this;
    }

    public Result LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        object?[]? propertyValues = null,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <inheritdoc />
    public Result Log(
        [ConstantExpected] string messageTemplate,
        Func<Result, object?[]> propertyValuesSelector,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(Log)
    )
    {
        var template = "Context: {Context} - Result: {Result} - Message: " + messageTemplate;
        var propertyValues = propertyValuesSelector(this);

        var allPropertyValues = new object?[propertyValues.Length + 2];
        allPropertyValues[0] = context;
        allPropertyValues[1] = ToString();
        propertyValues.CopyTo(allPropertyValues, 2);

        Serilog.Log.Write(logLevel, template, allPropertyValues);
        return this;
    }

    public Result LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        Func<Result, object?[]> propertyValues,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <inheritdoc />
    public static Result Try(
        Action action,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(Try)
    )
    {
        try
        {
            action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <inheritdoc />
    public static async Task<Result> TryAsync(
        Func<Task> action,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <inheritdoc />
    public static async Task<Result> TryAsync(
        Func<ValueTask> action,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <inheritdoc />
    public static Result MergeResults(params Result[] results)
    {
        if (results.Length == 0 || results.All(x => x.IsSuccess))
        {
            return Ok();
        }

        return Fail(results.SelectMany(x => x.Errors));
    }
}
