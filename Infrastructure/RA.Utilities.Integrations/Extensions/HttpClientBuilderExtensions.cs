using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Integrations.DelegatingHandlers;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Extensions methods for registering HTTP Clients and HttMessageHandlers.
/// </summary>
public static class HttpClientBuilderExtensions
{
    extension(IHttpClientBuilder httpClientBuilder)
    {
        /// <summary>
        /// Add <see cref="InternalHeadersForwardHandler"/> to add automatic
        /// access token and x-request-id for each HTTP call for this client.
        /// </summary>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used  to configure the client.</returns>
        public IHttpClientBuilder AddInternalHeadersForwardingHandler() =>
            httpClientBuilder.AddHttpMessageHandler<InternalHeadersForwardHandler>();

        /// <summary>
        /// Add <see cref="LoggingHandler"/> to add automatic logging outgoing HTTP requests
        /// and incoming HTTP responses.
        /// </summary>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used  to configure the client.</returns>
        public IHttpClientBuilder AddLoggingHandler() =>
            httpClientBuilder.AddHttpMessageHandler<LoggingHandler>();
    }
}
