using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Middlewares.Models;

internal sealed class BadRequestResponse
{
    public int ResponseCode { get; init; }
    public ResponseType ResponseType { get; init; }
    public string? ResponseMessage { get; init; }
    public BadRequestResult[] Result { get; init; }
}

internal sealed class BadRequestResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PropertyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? AttemptedValue { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ExpectedValue { get; set; }
}
