// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local


// ReSharper disable UnusedAutoPropertyAccessor.Global

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
    /// IMPORTANT: Order matters e.g. DateOnly and TimeOnly before DateTime.
    /// </summary>
    protected static readonly ImmutableArray<Type> ValidTypes =
    [
        typeof(Resource),
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(DateTime),
        typeof(Guid),
        typeof(double),
        typeof(bool),
        typeof(string),
    ];

    #region Constructors

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
    protected MergeTag(NonEmptyString name, MergeTagShortCode shortCode, NonEmptyString typeName)
    {
        Name = name;
        ShortCode = shortCode;
        TypeName = typeName;
        Type = GetSupportedType(typeName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeTag"/> class.
    /// </summary>
    /// <param name="name">The name of the merge tag.</param>
    /// <param name="type">The value type of the merge tag.</param>
    public MergeTag(NonEmptyString name, Type type)
        : this(name, MergeTagShortCode.Generate(name), type.Name) { }

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

    private Type GetSupportedType(string typeName) =>
        ValidTypes.FirstOrDefault(x => x.Name == typeName)
        ?? throw new ArgumentException($"Type {typeName} not supported.", nameof(typeName));
}
