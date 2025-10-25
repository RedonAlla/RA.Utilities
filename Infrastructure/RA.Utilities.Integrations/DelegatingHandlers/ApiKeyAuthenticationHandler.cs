using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.DelegatingHandlers;

/// <summary>
/// A delegating handler that retrieves an API key from a configured settings object
/// and adds it to the 'X-Api-Key' header of outgoing requests.
/// </summary>
/// <typeparam name="TSettings">The type of the settings object that implements <see cref="IApiKeySettings"/>.</typeparam>
public class ApiKeyAuthenticationHandler<TSettings> : DelegatingHandler
    where TSettings : class, IApiKeySettings
{
    private readonly TSettings? _settings;
    private const string xApiKeyHeader = "X-Api-Key";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler{TSettings}"/> class.
    /// </summary>
    /// <param name="options">The options accessor to retrieve the settings containing the API key.</param>
    public ApiKeyAuthenticationHandler(IOptions<TSettings> options)
    {
        _settings = options?.Value;
    }

    /// <summary>
    /// Adds the API key to the request header and sends the request to the inner handler.
    /// </summary>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_settings?.ApiKey))
        {
            request?.Headers.TryAddWithoutValidation(xApiKeyHeader, _settings.ApiKey);
        }

        return base.SendAsync(request!, cancellationToken);
    }
}
