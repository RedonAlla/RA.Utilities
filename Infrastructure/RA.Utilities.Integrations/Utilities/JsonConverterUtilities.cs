using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RA.Utilities.Integrations.Utilities;

/// <summary>
/// Provides utility methods for JSON serialization and deserialization.
/// </summary>
internal static class JsonConverterUtilities
{
    /// <summary>
    /// Gets the default JSON serializer options used throughout the application.
    /// Configured for camel-case property names, indented writing, and ignoring null values.
    /// </summary>
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Serializes an object to a JSON string using default settings.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to serialize.</typeparam>
    /// <param name="request">The object to serialize.</param>
    /// <returns>A JSON string representation of the object, or an empty string if the object is null.</returns>
    public static string ToJsonString<TObject>(TObject? request) =>
        request is null ? string.Empty : JsonSerializer.Serialize(request, DefaultJsonSerializerOptions);

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to deserialize to.</typeparam>
    /// <param name="jsonString">The JSON string content to deserialize.</param>
    /// <returns>The deserialized object, or the default value for <typeparamref name="TObject"/> if the input string is null or whitespace.</returns>
    public static TObject? ToObject<TObject>(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return default;
        }

        return JsonSerializer.Deserialize<TObject>(jsonString, DefaultJsonSerializerOptions);
    }

    /// <summary>
    /// Converts the request body to a JSON <see cref="StringContent"/> representation.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representation of the request body, suitable for use as HTTP content.</returns>
    public static StringContent ToStringContent<TObject>(TObject value) where TObject : class =>
        new(ToJsonString(value), Encoding.UTF8, MediaTypeNames.Application.Json);
}
