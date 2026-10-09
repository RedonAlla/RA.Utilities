namespace RA.Utilities.Logging.Shared.Constants;

/// <summary>
/// Defines constants used for logging purposes.
/// </summary>
public static class LoggingConstants
{
    /// <summary>
    /// A logging parameter added by Logger, it is x-request-id header parameter
    /// </summary>
    public const string XRequestId = "x-request-id";

    /// <summary>
    /// A logging parameter added by Logger, it is HttpContext.TraceIdentifier.
    /// And it is the same for all logs in that call.
    /// Useful to trace log in this http call.
    /// </summary>
    public const string TraceId = "TraceId";
}
