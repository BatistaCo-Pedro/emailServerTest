// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace App.Server.Notification.Application.Domain.Entities.JsonEntities;

/// <summary>
/// Base class for merge tags.
/// </summary>
/// <remarks>
/// Merge tags should be a pure value object and not be tracked
/// - due to current ef core limitations it must be configured as an Owned entity.
/// </remarks>
/// TODO: Update merge tag configuration to ComplexType as soon as https://github.com/dotnet/efcore/issues/31252 gets merged.
public record MergeTag : IJsonEntity
{
    /// <summary>
    /// Valid types for JSON values.
    /// IMPORTANT: Valid types should only be primitive types and the order matters e.g. DateOnly and TimeOnly before DateTime.
    /// </summary>
    protected static readonly ImmutableArray<Type> ValidTypes =
    [
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(DateTime),
        typeof(Guid),
        typeof(double),
        typeof(bool),
        typeof(string),
    ];

    #region Constructors

    [Obsolete("Required by DI and EF Core")]
    protected MergeTag() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeTag"/> class.
    /// </summary>
    /// <param name="type">The type of the merge tag.</param>
    /// <exception cref="ArgumentException">Type is not primitive type.</exception>
    /// <remarks>
    /// Common constructor to be used by other constructors.
    /// </remarks>
    private MergeTag(Type type)
    {
        ValidateTypeForData(type);
        Type = type;
        TypeName =
            type.FullName
            ?? throw new ArgumentException(
                $"'FullName' of type {Type.Name} is null, type is not a primitive type.",
                Type.Name
            ); // This exception should never be thrown unless when debugging.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeTag"/> class.
    /// Used by the JSON deserializer.
    /// </summary>
    /// <param name="name">The name of the merge tag.</param>
    /// <param name="shortCode">The short code of the merge tag.</param>
    /// <param name="typeName">The name of the value type of the merge tag.</param>
    /// <exception cref="ArgumentException">Type couldn't be found.</exception>
    /// <remarks>
    /// Constructor used to deserialize to an object. Protected to prevent direct use.
    /// </remarks>
    [JsonConstructor]
    public MergeTag(NonEmptyString name, MergeTagShortCode shortCode, NonEmptyString typeName)
        : this(GetTypeByName(typeName))
    {
        Name = name;
        ShortCode = shortCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeTag"/> class.
    /// </summary>
    /// <param name="name">The name of the merge tag.</param>
    /// <param name="type">The value type of the merge tag.</param>
    public MergeTag(NonEmptyString name, Type type)
        : this(type)
    {
        Name = name;
        ShortCode = MergeTagShortCode.Generate(name);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The name of the merge tag.
    /// </summary>
    [JsonPropertyName("name")]
    public NonEmptyString Name { get; private init; }

    /// <summary>
    /// The value of the merge tag represented as a string.
    /// Can be any JSON primitive value except for arrays, objects or NULL.
    /// </summary>
    [JsonPropertyName("shortCode")]
    public MergeTagShortCode ShortCode { get; private init; }

    /// <summary>
    /// The type of the merge tag represented as a string.
    /// </summary>
    /// <exception cref="InvalidOperationException">Must be set before being accessed.</exception>
    /// <remarks>
    /// Can only be sent once per merge tag object.
    /// </remarks>
    [JsonPropertyName("typeName")]
    public NonEmptyString TypeName { get; private init; }

    /// <summary>
    /// The type of the merge tag.
    /// </summary>
    /// <exception cref="InvalidOperationException">Must be set before being accessed.</exception>
    /// <remarks>
    /// Can only be sent once per merge tag object.
    /// </remarks>
    [JsonIgnore]
    [NotMapped]
    public Type Type { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a type by its name.
    /// </summary>
    /// <param name="typeName">The type name to attempt to get the type for.</param>
    /// <returns>The type if it was found, otherwise null.</returns>
    /// <exception cref="ArgumentException">Type couldn't be found.</exception>
    private static Type GetTypeByName(NonEmptyString typeName) =>
        Type.GetType(typeName)
        ?? throw new ArgumentException($"Type {typeName} not found.", nameof(typeName));

    /// <summary>
    /// Validates if the type is valid for merge tags.
    /// </summary>
    /// <param name="type">The type to be validated.</param>
    /// <exception cref="ArgumentException">Type is not valid to be used for merge tags.</exception>
    private static void ValidateTypeForData(Type? type)
    {
        if (type is null || !ValidTypes.Contains(type))
        {
            throw new ValidationException(
                $"Type {type?.FullName ?? type?.Name} is not a valid type to be used for {nameof(MergeTag)}."
            );
        }
    }

    #endregion
}