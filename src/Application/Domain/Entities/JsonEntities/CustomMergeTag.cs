#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace App.Server.Notification.Application.Domain.Entities.JsonEntities;

/// <summary>
/// Represents an actual data element.
/// Supposed to be serialized and deserialized to JSON.
/// </summary>
[Serializable]
public record CustomMergeTag : MergeTag
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMergeTag"/> class.
    /// Used by the JSON deserializer.
    /// </summary>
    /// <param name="stringValue">The value of the merge tag as a string.</param>
    /// <param name="name">The name of the merge tag.</param>
    /// <param name="shortCode">The short code of the merge tag.</param>
    /// <param name="typeName">The name of the value type of the merge tag.</param>
    /// <exception cref="ArgumentException">Type couldn't be found.</exception>
    /// <remarks>
    /// Constructor used to serialize and deserialize the object. Protected to prevent direct use.
    /// Mostly for serialization and deserialization between the database and the application.
    /// </remarks>
    [JsonConstructor]
    protected CustomMergeTag(
        NonEmptyString stringValue,
        NonEmptyString name,
        MergeTagShortCode shortCode,
        NonEmptyString typeName
    )
        : base(name, shortCode, typeName)
    {
        StringValue = stringValue;
        Value = StringHelper.Parse(stringValue, Type);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMergeTag"/> class.
    /// </summary>
    /// <param name="name">The name of the merge tag.</param>
    /// <param name="typeName"></param>
    /// <param name="stringValue">The value of the element.</param>
    /// <remarks>
    /// Constructor used by the mapper to create a new instance with an arbitrary value.
    /// </remarks>
    public CustomMergeTag(NonEmptyString name, NonEmptyString typeName, NonEmptyString stringValue)
        : this(stringValue, name, MergeTagShortCode.Generate(name), typeName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMergeTag"/> class.
    /// </summary>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The value of the element.</param>
    /// <exception cref="ArgumentException">Value couldn't be converted to string.</exception>
    public CustomMergeTag(NonEmptyString name, object value)
        : base(name, value.GetType())
    {
        StringValue =
            Convert.ToString(value, CultureInfo.InvariantCulture)
            ?? throw new ArgumentException(
                $"Value {value} couldn't be converted to string.",
                nameof(value)
            );
        Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The value of the data element represented as a string.
    /// Can be any JSON value.
    /// </summary>
    [JsonPropertyName("stringValue")]
    public NonEmptyString StringValue { get; private init; }

    /// <summary>
    /// The value of the data element represented as its actual type.
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    public object Value { get; }

    /// <summary>
    /// Gets a type by its name.
    /// </summary>
    /// <param name="typeName">The type name to attempt to get the type for.</param>
    /// <returns>The type if it was found, otherwise null.</returns>
    /// <exception cref="ArgumentException">Type couldn't be found.</exception>
    private static Type GetTypeByName(NonEmptyString typeName) =>
        Type.GetType(typeName)
        ?? throw new ArgumentException($"Type {typeName} not found.", nameof(typeName));

    #endregion
}
