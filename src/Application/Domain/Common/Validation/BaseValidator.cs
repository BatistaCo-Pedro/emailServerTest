namespace App.Server.Notification.Application.Domain.Common.Validation;

/// <summary>
/// Base class for validators.
/// </summary>
/// <typeparam name="T">The type of the object being validated.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseValidator{T}"/> class.
/// </remarks>
/// <remarks>
/// This could (should) be improved and extended.
/// </remarks>
public abstract class BaseValidator<T>
{
    /// <summary>
    /// Object being validated.
    /// </summary>
    protected readonly T Obj;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseValidator{T}"/> class.
    /// </summary>
    /// <param name="obj">The object to be validated.</param>
    // ReSharper disable once ConvertToPrimaryConstructor - Easier to read like this.
    protected BaseValidator(T obj)
    {
        Obj = obj;
    }

    /// <summary>
    /// Validates the object.
    /// </summary>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> of <see cref="ValidationResult"/> with the validation results.</returns>
    public virtual List<ValidationResult> Validate()
    {
        if (Obj is null)
        {
            return [];
        }

        var context = new ValidationContext(Obj);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(Obj, context, results, true);

        return results;
    }

    /// <summary>
    /// Validates the object and throws a <see cref="ValidationException"/> if a validation doesn't pass.
    /// </summary>
    /// <exception cref="ValidationException">Object is not valid.</exception>
    public virtual void ValidateAndThrow()
    {
        if (Obj is null)
        {
            return;
        }

        var context = new ValidationContext(Obj);
        Validator.ValidateObject(Obj, context, true);
    }
}
