namespace App.Server.Notification.Application.Domain.Types.Attributes;

/// <summary>
/// Validation attribute checking whether
/// - string is null, whitespace or empty
/// - enumerable is empty
/// - value is null
/// </summary>
public class NotEmptyAttribute : ValidationAttribute
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value switch
        {
            string stringValue => !string.IsNullOrWhiteSpace(stringValue),
            IEnumerable enumerable => enumerable.Cast<object>().Any(),
            _ => value is not null,
        };
    }
}
