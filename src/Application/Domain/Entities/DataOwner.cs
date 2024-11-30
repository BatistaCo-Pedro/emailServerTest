#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace App.Server.Notification.Application.Domain.Entities;

/// <summary>
/// The data owner is an outside entity that owns presets and custom templates.
/// This can be anything from a hotel to a restaurant, or even a person.
/// </summary>
public class DataOwner : Entity
{
    [Obsolete("Required by EF Core")]
    protected DataOwner() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataOwner"/> class.
    /// </summary>
    /// <param name="name">The name of the data owner.</param>
    /// <param name="source">The source of the data owner.</param>
    public DataOwner(NonEmptyString name, NonEmptyString source)
    {
        Name = name;
        Source = source;
    }

    /// <summary>
    /// The name of the owner of the data.
    /// </summary>
    [StringLength(50)]
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// The source of the owner entity.
    /// </summary>
    [StringLength(50)]
    public NonEmptyString Source { get; private set; }
}
