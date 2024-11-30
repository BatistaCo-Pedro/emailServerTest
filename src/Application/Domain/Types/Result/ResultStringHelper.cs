namespace App.Server.Notification.Application.Domain.Types.Result;

/// <summary>
/// Helper class for creating strings representing results and errors.
/// </summary>
internal static class ResultStringHelper
{
    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <param name="successString">The success string representation.</param>
    /// <param name="informationString">A string with additional information.</param>
    /// <returns>A string representation of the result object.</returns>
    public static string GetResultString(
        string typeName,
        string successString,
        string informationString
    )
    {
        const string preResultStr = " { IsSuccess = ";
        const string separator = ", ";
        const string postResultStr = " }";
        var stringLength =
            typeName.Length
            + preResultStr.Length
            + successString.Length
            + separator.Length
            + informationString.Length
            + postResultStr.Length;

        return string.Create(
            stringLength,
            (typeName, successString, informationString),
            (span, state) =>
            {
                span.TryWrite(
                    $"{state.typeName}{preResultStr}{state.successString}{separator}{state.informationString}{postResultStr}",
                    out _
                );
            }
        );
    }

    /// <summary>
    /// Returns a string representation of the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A string representation of the value.</returns>
    public static string GetResultValueString<T>(T value)
    {
        var valueString = value?.ToString() ?? string.Empty;

        const string preValueStr = ", Value = ";

        var stringLength = preValueStr.Length + valueString.Length;

        return string.Create(
            stringLength,
            valueString,
            (span, state) =>
            {
                span.TryWrite($"{preValueStr}{state}", out _);
            }
        );
    }

    /// <summary>
    /// Returns a string representation of the first error in the collection.
    /// </summary>
    /// <param name="errors">The error collection.</param>
    /// <returns>A string representation of the first error in the collection.</returns>
    public static string GetResultErrorString(ImmutableList<IError> errors)
    {
        var errorMessages = errors.Select(x => x.ToString());

        return string.Join(", ", errorMessages);
    }

    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A string representation of the error.</returns>
    public static string GetErrorString(IError error)
    {
        var errorType = error.GetType().Name;

        if (error.Message.Length <= 0)
            return errorType;

        var errorMessage = error.Message;

        const string preErrorStr = " { Message = \"";
        const string postErrorStr = "\" }";
        var stringLength =
            errorType.Length + preErrorStr.Length + errorMessage.Length + postErrorStr.Length;

        return string.Create(
            stringLength,
            (errorType, errorMessage),
            (span, state) =>
            {
                span.TryWrite(
                    $"{state.errorType}{preErrorStr}{state.errorMessage}{postErrorStr}",
                    out _
                );
            }
        );
    }
}
