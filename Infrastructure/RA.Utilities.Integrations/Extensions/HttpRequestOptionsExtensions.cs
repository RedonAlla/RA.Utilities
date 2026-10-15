using System.Linq;
using System.Net.Http;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequestOptions"/> and related types.
/// </summary>
public static class HttpRequestOptionsExtensions
{
    private const string HttpContext = "MS_HttpContext";
    private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

    extension(HttpRequestOptions httpRequestOptions)
    {
        /// <summary>
        /// Get client IP Address.
        /// </summary>
        /// <returns></returns>
        public string GetClientIpAddress()
        {
            var options = httpRequestOptions.ToDictionary(header => header.Key, header => header.Value);

            if (options.TryGetValue(HttpContext, out object? value))
            {
                dynamic ctx = value;

                if (ctx != null)
                    return ctx.Request.UserHostAddress;
            }

            if (options.TryGetValue(RemoteEndpointMessage, out object? value1))
            {
                dynamic remoteEndpoint = value1;

                if (remoteEndpoint != null)
                    return remoteEndpoint.Address;
            }

            return string.Empty;
        }
    }
}
