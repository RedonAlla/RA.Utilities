using System;
using System.ComponentModel.DataAnnotations;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.Models;

/// <summary>
/// Represents the base settings for an HTTP client, including base URL, proxy usage, and timeout.
/// This ensures that any settings class used with the integration helpers
/// provides the necessary properties to configure an HttpClient.
/// </summary>
public class BaseHttpClientSettings<T> : IIntegrationSettings
{
    /// <summary>
    /// Base URL for the HTTP client.
    /// This property is required and must be a valid URL format.
    /// </summary>
    /// <value>
    /// The base address <see cref="Uri"/> of the internet resource used when sending requests.
    /// </value>
    [Required]
    public Uri BaseUrl { get; set; }

    /// <summary>
    /// The actions specific to the HTTP client.
    /// </summary>
    [Required]
    public T Actions { get; set; }

    /// <summary>
    /// A value indicating whether a proxy should be used for requests.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the handler should use a proxy for requests; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool UseProxy { get; set; }

    /// <summary>
    /// Request timeout in seconds.
    /// The value must be between 1 and 600 seconds. Defaults to 100.
    /// </summary>
    /// <value>
    /// The number of seconds to wait for a response. The default is 100.
    /// </value>
    /// <remarks>
    /// For more information, see the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-8.0">HttpClient.Timeout documentation</see>.
    /// </remarks>
    public double Timeout { get; set; } = 100;
}
