using System.Text.Json.Serialization;

namespace RA.Utilities.Api.Results;

/// <summary>
/// Represents a result containing error information.
/// </summary>
public class ErrorResult
{
    /// <summary>
    /// The machine-readable error code associated with this specific error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorCode { get; set; }

    /// <summary>
    /// The human-readable description of the error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; }
}
