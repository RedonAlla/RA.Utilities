using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequestMessage"/> and related types.
/// </summary>
public static class HttpRequestMessageExtensions
{
    private const string HttpContext = "MS_HttpContext";
    private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

    /// <summary>
    /// Get client IP Address.
    /// </summary>
    /// <param name="httpRequestOptions"></param>
    /// <returns></returns>
    public static string GetClientIpAddress(this HttpRequestOptions httpRequestOptions)
    {
        Dictionary<string, object> options = httpRequestOptions!.ToDictionary(header => header.Key, header => header.Value)!;

        if (options.TryGetValue(HttpContext, out object? value))
        {
            dynamic ctx = value;
            if (ctx != null)
            {
                return ctx.Request.UserHostAddress;
            }
        }

        if (options.TryGetValue(RemoteEndpointMessage, out object? value1))
        {
            dynamic remoteEndpoint = value1;
            if (remoteEndpoint != null)
            {
                return remoteEndpoint.Address;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Converts <see cref="HttpHeaders"/> in to <see cref="IDictionary{String, String}"/>.
    /// </summary>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static Dictionary<string, string>? ToDictionary(this HttpHeaders headers) =>
        headers.ToDictionary(header => header.Key, header => string.Join(",", header.Value));
}
