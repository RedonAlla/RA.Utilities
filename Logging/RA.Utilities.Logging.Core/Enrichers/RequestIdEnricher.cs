using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Logging.Core.Extensions;
using Serilog.Core;
using Serilog.Events;

namespace RA.Utilities.Logging.Core.Enrichers;

/// <summary>
/// Enriches log events with a request ID property.
/// The ID is determined by checking the following sources in order:
/// <list type="number">
/// <item><description>The <c>x-request-id</c> header from the current HTTP request.</description></item>
/// <item><description>The <see cref="HttpContext.TraceIdentifier"/> from the current HTTP request.</description></item>
/// <item><description>The ID from the current <see cref="System.Diagnostics.Activity"/>.</description></item>
/// </list>
/// This enricher depends on <see cref="IHttpContextAccessor"/> to access the HTTP context
/// and should be registered in the dependency injection container.
/// </summary>
/// <seealso cref="ILogEventEnricher" />
public class RequestIdEnricher : ILogEventEnricher
{
    /// <summary>
    /// The property name added to enriched log events.
    /// </summary>
    private const string RequestIdPropertyName = "XRequestId";

    private const string TraceIdPropertyName = "TraceId";

    /// <summary>
    /// The name of the header to check for a request ID.
    /// </summary>
    private const string HeadersXRequestIdPropertyName = "x-request-id";

    private readonly HttpContext _httpContext;

    /// <summary>
    /// Initialize a new instance of <see cref="RequestIdEnricher"/>
    /// with default <see cref="HttpContextAccessor"/>.
    /// Initializes a new instance of the <see cref="RequestIdEnricher"/> class.
    /// </summary>
    public RequestIdEnricher() : this(new HttpContextAccessor())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIdEnricher"/> class.
    /// </summary>
    /// <param name="contextAccessor">The HTTP context accessor, used to retrieve request-specific information.
    /// This is typically injected by the dependency injection container.</param>
    public RequestIdEnricher(IHttpContextAccessor contextAccessor)
    {
        _httpContext = contextAccessor.HttpContext!;
    }

    /// <summary>
    /// Enriches the log event with a request ID if one can be found.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string? requestId = _httpContext?.Request.Headers[HeadersXRequestIdPropertyName];

        if (!string.IsNullOrEmpty(requestId))
        {
            LogEventProperty property = propertyFactory.CreateProperty(RequestIdPropertyName, requestId);
            logEvent.AddPropertyIfAbsent(property);
        }

        string traceId = _httpContext?.TraceIdentifier ?? Activity.Current?.GetActivityId();

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            LogEventProperty property = propertyFactory.CreateProperty(TraceIdPropertyName, traceId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
