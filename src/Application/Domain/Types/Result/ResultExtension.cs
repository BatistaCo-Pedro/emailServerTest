namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Partial class of <see cref="Result"/> for extensions.
/// </summary>
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
public sealed partial class Result : IAuditableResult<Result>
{
    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result Log(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[]? propertyValues
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
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[]? propertyValues
    ) => IsSuccess ? Log(logLevel, context, messageTemplate, propertyValues) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[]? propertyValues
    ) => !IsSuccess ? Log(logLevel, context, messageTemplate, propertyValues) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result Log(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result, object?[]> propertyValuesSelector
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
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result, object?[]> propertyValuesSelector
    ) => IsSuccess ? Log(logLevel, context, messageTemplate, propertyValuesSelector) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result, object?[]> propertyValuesSelector
    ) => !IsSuccess ? Log(logLevel, context, messageTemplate, propertyValuesSelector) : this;

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
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
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
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
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
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
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
