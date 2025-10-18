using System;
using System.Net;
using System.Net.Http;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.DelegatingHandlers;

/// <summary>
/// A factory for creating an <see cref="HttpClientHandler"/> configured with proxy settings.
/// This is not a DelegatingHandler because proxy settings must be applied to the primary message handler.
/// </summary>
internal static class ProxyMessageHandler
{
    /// <summary>
    /// Creates a new <see cref="HttpClientHandler"/> instance based on the provided proxy settings.
    /// </summary>
    /// <param name="proxySettings">The proxy configuration settings.</param>
    /// <returns>A configured <see cref="HttpClientHandler"/>.</returns>
    public static HttpClientHandler Create(IProxySettings? proxySettings)
    {
        if (proxySettings is null || string.IsNullOrWhiteSpace(proxySettings.Address))
        {
            // No proxy settings provided, return a default handler.
            return new HttpClientHandler();
        }

        var proxy = new WebProxy(new Uri(proxySettings.Address))
        {
            BypassProxyOnLocal = proxySettings.BypassProxyOnLocal
        };

        if (!string.IsNullOrWhiteSpace(proxySettings.Username))
        {
            proxy.Credentials = new NetworkCredential(proxySettings.Username, proxySettings.Password);
        }

        return new HttpClientHandler { Proxy = proxy, UseProxy = true };
    }
}
