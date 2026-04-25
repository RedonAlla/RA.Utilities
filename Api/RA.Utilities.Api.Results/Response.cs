using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// A class encapsulating API response with success or failure indicators.
/// </summary>
/// <typeparam name="T">The type of the result data contained within the response.</typeparam>
public class Response<T>
{
    /// <summary>
    /// The response code. This can be an HTTP status code or a domain-specific error code for client-side processing.
    /// </summary>
    public int ResponseCode { get; set; }

    /// <summary>
    /// The semantic type of the response, used to categorize the outcome (e.g., Success, BadRequest, Error).
    /// </summary>
    public ResponseType ResponseType { get; set; }

    /// <summary>
    /// A human-friendly message describing the result of the API operation.
    /// </summary>
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// Actual API response of type <typeparamref name="T"/>.
    /// </summary>
    public T? Result { get; set; }
}
