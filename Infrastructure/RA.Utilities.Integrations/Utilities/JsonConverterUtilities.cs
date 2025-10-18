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
    /// Deserializes a string to an object of the specified type based on the media type.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to deserialize to.</typeparam>
    /// <param name="jsonString">The string content to deserialize.</param>
    /// <param name="mediaType">The media type of the content. Currently, only "application/json" is supported.</param>
    /// <returns>The deserialized object, or the default value for <typeparamref name="TObject"/> if the input string is null or whitespace.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported media type is provided.</exception>
    public static TObject? ToObject<TObject>(string jsonString, string mediaType)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return default;
        }

        return mediaType switch
        {
            MediaTypeNames.Application.Json => JsonSerializer.Deserialize<TObject>(jsonString, DefaultJsonSerializerOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(mediaType), $"Unsupported media type: {mediaType}"),
        };
    }

    /// <summary>
    /// Converts the request body to a string content representation based on the specified media type.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="mediaType">The media type for the content (e.g., "application/json").</param>
    /// <param name="encoding">The character encoding for the content.</param>
    /// <returns>A string representation of the request body, suitable for use as HTTP content.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported media type is provided.</exception>
    public static StringContent ToStringContent<TObject>(TObject value, string mediaType, Encoding encoding) where TObject : class => mediaType switch
    {
        MediaTypeNames.Application.Json => new StringContent(ToJsonString(value), encoding, mediaType),
        _ => throw new ArgumentOutOfRangeException(nameof(mediaType)),
    };
}
