namespace App.Server.Notification.Application.Domain.Common.Validation;

/// <summary>
/// Interface for a static validator.
/// </summary>
/// <typeparam name="T">The type of the object being validated.</typeparam>
public interface IStaticValidator<in T>
{
    /// <summary>
    /// Validates the object.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> of <see cref="ValidationResult"/> with the validation results.</returns>
    public static abstract IReadOnlyCollection<ValidationResult> Validate(T obj);

    /// <summary>
    /// Validates the object and throws a <see cref="ValidationException"/> if a validation doesn't pass.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <exception cref="ValidationException">Object is not valid.</exception>
    public static abstract void ValidateAndThrow(T obj);
}
