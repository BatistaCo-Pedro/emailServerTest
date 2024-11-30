namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// Converter used during serialization and deserialization to convert strings directly into a string wrapper and back.
/// </summary>
/// <typeparam name="TWrapper">The type of the string wrapper.</typeparam>
public sealed class StringToWrapperJsonConverter<TWrapper> : JsonConverter<TWrapper>
    where TWrapper : class, IStringWrapper<TWrapper>
{
    /// <summary>
    /// Deserializes a string wrapper from a JSON string.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The conversion options.</param>
    /// <returns>The string wrapper.</returns>
    /// <exception cref="SerializationException">String is malformed and couldn't be read.</exception>
    public override TWrapper Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) =>
        TWrapper.Create(
            reader.GetString() ?? throw new SerializationException("Couldn't read string")
        );

    /// <summary>
    /// Serializes a string wrapper into a string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="wrapper">The object wrapper to serialize.</param>
    /// <param name="options">The conversion options.</param>
    /// <exception cref="InvalidCastException">Wrapper couldn't be cast to string.</exception>
    public override void Write(
        Utf8JsonWriter writer,
        TWrapper wrapper,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(wrapper.Value);
}
