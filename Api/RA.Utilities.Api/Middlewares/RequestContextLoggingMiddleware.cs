using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RA.Utilities.Api.Utilities;
using RA.Utilities.Logging.Shared.Constants;

namespace RA.Utilities.Api.Middlewares;

/// <summary>
/// Middleware to enrich log entries with request-specific context, such as a correlation ID.
/// </summary>
public class RequestContextLoggingMiddleware(ILogger<RequestContextLoggingMiddleware> logger) : IMiddleware
{
    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var loggerScope = new Dictionary<string, object>
        {
            [LoggingConstants.XRequestId] = CommonUtilities.GetRequestId(context),
        };

        using (logger.BeginScope(loggerScope))
        {
            await next(context);
        }
    }
}
