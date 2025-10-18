
using System.Collections.Generic;

namespace RA.Utilities.Logging.Shared.Models.HttpLog;

/// <summary>
/// Model for logging HTTP response.
/// </summary>
public class HttpResponseLogTemplate : BaseHttpLogTemplate
{
    /// <summary>
    /// The collection of HTTP response headers.
    /// </summary>
    public IDictionary<string, string>? ResponseHeaders { get; set; }

    /// <summary>
    ///The status code of the HTTP response.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    ///HTTP response body.
    /// </summary>
    public object? ResponseBody { get; set; }

    /// <summary>
    /// The total time taken to process the request and generate the response.
    /// </summary>
    public string Duration { get; set; }
}
