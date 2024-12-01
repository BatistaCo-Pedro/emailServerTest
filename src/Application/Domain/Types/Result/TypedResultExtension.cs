namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Partial class of <see cref="Result{TValue}"/> for extensions.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
public sealed partial class Result<TValue> : IAuditableResult<Result<TValue>>
{
    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result<TValue> Log(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[] propertyValues
    )
    {
        var template = "Context: {Context} - Result: {Result} - Message: " + messageTemplate;
        var allPropertyValues = new object?[propertyValues.Length + 2];
        allPropertyValues[0] = context;
        allPropertyValues[1] = ToString();
        propertyValues.CopyTo(allPropertyValues, 2);

        Serilog.Log.Write(logLevel, template, allPropertyValues);
        return this;
    }

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result<TValue> LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[] propertyValues
    ) => IsSuccess ? Log(logLevel, context, messageTemplate, propertyValues) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result<TValue> LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        [ConstantExpected] string messageTemplate,
        params object?[] propertyValues
    ) => !IsSuccess ? Log(logLevel, context, messageTemplate, propertyValues) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result<TValue> Log(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValuesSelector
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
    public Result<TValue> LogIfSuccess(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValuesSelector
    ) => IsSuccess ? Log(logLevel, context, messageTemplate, propertyValuesSelector) : this;

    /// <inheritdoc />
    [MessageTemplateFormatMethod(nameof(messageTemplate))]
    public Result<TValue> LogIfFailure(
        in LogEventLevel logLevel,
        string context,
        string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValuesSelector
    ) => !IsSuccess ? Log(logLevel, context, messageTemplate, propertyValuesSelector) : this;

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
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex))
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
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
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex))
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
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
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex))
                .Log(LogEventLevel.Error, context, "Exception: {Exception}", ex.Message);
        }
    }
}
