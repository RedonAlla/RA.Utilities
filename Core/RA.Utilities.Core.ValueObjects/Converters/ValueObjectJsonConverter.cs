using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RA.Utilities.Core.ValueObjects.Converters;

/// <summary>
/// Serializes an <see cref="IValueObject{TSelf}"/> as a plain JSON string and validates it when deserializing.
/// </summary>
/// <typeparam name="T">The value object type to convert.</typeparam>
public sealed class ValueObjectJsonConverter<T> : JsonConverter<T>
    where T : IValueObject<T>
{
    /// <summary>
    /// Reads a JSON string and converts it to a <typeparamref name="T"/> via <see cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>,
    /// so the value object's own validation runs during deserialization.
    /// </summary>
    /// <param name="reader">The reader positioned on the value to read.</param>
    /// <param name="typeToConvert">The type being converted (always <typeparamref name="T"/>).</param>
    /// <param name="options">The serializer options in use.</param>
    /// <returns>The validated <typeparamref name="T"/>, or <see langword="default"/> for an empty string.</returns>
    /// <exception cref="JsonException">The token is not a string, or the string is not a valid <typeparamref name="T"/>.</exception>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"{typeof(T).Name} must be a JSON string.");

        string? stringValue = reader.GetString();

        if (string.IsNullOrWhiteSpace(stringValue))
            return default;

        try
        {
            return T.Parse(stringValue, provider: null);
        }
        catch (Exception ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    /// <summary>
    /// Writes the <typeparamref name="T"/> as a JSON string, for example <c>"EUR"</c>.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value object to write.</param>
    /// <param name="options">The serializer options in use.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
