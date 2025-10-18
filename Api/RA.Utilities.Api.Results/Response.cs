using System;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Results;

/// <summary>
/// A class encapsulating API response with success or failure indicators.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Response<T>
{
    /// <summary>
    /// Response code indicates error code for translation purposes.
    /// </summary>
    public int ResponseCode { get; set; }

    /// <summary>
    /// Response type for response.
    /// </summary>
    public ResponseType ResponseType { get; set; }

    /// <summary>
    /// A short and human-friendly message about response.
    /// </summary>
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// Actual API response of type <typeparamref name="T"/>.
    /// </summary>
    public T? Result { get; set; }
}
