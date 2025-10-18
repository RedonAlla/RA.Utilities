using System;

namespace RA.Utilities.Core.Constants;

/// <summary>
/// Defines common HTTP header parameter names used across services.
/// </summary>
public static class HeaderParameters
{
    /// <summary>
    /// Represents the 'x-request-id' header, used for request correlation and tracing.
    /// </summary>
    public const string XRequestId = "x-request-id";

    /// <summary>
    /// Represents the 'trace-id' header, used for internal tracing.
    /// </summary>
    public const string TraceId = "trace-id";

    /// <summary>
    /// Represents the 'Location' header, used in responses to redirect or indicate the location of a new resource.
    /// </summary>
    public const string Location = "location";

    /// <summary>
    /// Represents the 'Authorization' header, used for sending authentication credentials.
    /// </summary>
    public const string Authorization = "Authorization";
}
