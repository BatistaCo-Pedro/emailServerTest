namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Partial class of <see cref="Result{TValue}"/> for extensions.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
public sealed partial class Result<TValue> : IAuditableResult<Result<TValue>>
{
    /// <inheritdoc />
    public Result<TValue> Log(
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

    /// <inheritdoc />
    public Result<TValue> LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        object?[]? propertyValues = null,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <inheritdoc />
    public Result<TValue> Log(
        [ConstantExpected] string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValuesSelector,
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

    /// <inheritdoc />
    public Result<TValue> LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValues,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <inheritdoc />
    public static Result<TValue> Try(
        Func<TValue> func,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(Try)
    )
    {
        try
        {
            return Ok(func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <inheritdoc />
    public static async Task<Result<TValue>> TryAsync(
        Func<Task<TValue>> func,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <inheritdoc />
    public static async Task<Result<TValue>> TryAsync(
        Func<ValueTask<TValue>> func,
        Func<Exception, IError>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }
}
