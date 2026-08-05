using System.Text.Json.Serialization;

namespace RA.Utilities.Api.Json;

/// <summary>
/// Provides a JSON serialization context for HTTP logging.
/// </summary>
[JsonSerializable(typeof(object))]
internal sealed partial class HttpLoggingJsonContext : JsonSerializerContext
{
}
