using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Integrations.DelegatingHandlers;

/// <summary>
/// A <see cref="DelegatingHandler"/> that forwards the Authorization token and x-request-id from an incoming request to an outgoing request.
/// This is typically used for internal service-to-service calls to propagate authentication and tracing information.
/// </summary>
/// <seealso cref="System.Net.Http.DelegatingHandler" />
public class InternalHeadersForwardHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalHeadersForwardHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The accessor for the current <see cref="HttpContext"/>.</param>
    /// <remarks>
    /// This handler retrieves the Authorization token, x-request-id, and trace identifier from the current <see cref="HttpContext"/>.
    /// If the context or the headers are not available, the corresponding values will be empty strings to avoid runtime exceptions.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpContextAccessor"/> is <see langword="null"/>.</exception>
    public InternalHeadersForwardHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Forwards the Authorization token and x-request-id in the outgoing request headers and sends the request.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="HttpResponseMessage"/> from the server.
    /// </returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string token = _httpContextAccessor.HttpContext?.Request.Headers[HeaderParameters.Authorization].ToString();
        string requestId = _httpContextAccessor.HttpContext?.Request.Headers[HeaderParameters.XRequestId].ToString();
        string traceIdentifier = _httpContextAccessor.HttpContext?.TraceIdentifier;

        request.Headers.TryAddWithoutValidation(HeaderParameters.Authorization, token);
        request.Headers.TryAddWithoutValidation(HeaderParameters.XRequestId, requestId ?? traceIdentifier);

        return await base.SendAsync(request, cancellationToken);
    }

}
