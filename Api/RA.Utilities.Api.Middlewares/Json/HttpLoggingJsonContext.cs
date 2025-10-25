using System.Text.Json.Serialization;

namespace RA.Utilities.Api.Middlewares.Json;

/// <summary>
/// Provides a JSON serialization context for HTTP logging.
/// </summary>
[JsonSerializable(typeof(object))]
public partial class HttpLoggingJsonContext : JsonSerializerContext
{
}
