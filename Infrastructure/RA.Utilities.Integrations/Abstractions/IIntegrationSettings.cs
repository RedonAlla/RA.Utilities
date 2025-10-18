using System;

namespace RA.Utilities.Integrations.Abstractions;

/// <summary>
/// Defines the basic contract for HTTP client integration settings.
/// This ensures that any settings class used with the integration helpers
/// provides the necessary properties to configure an HttpClient.
/// </summary>
public interface IIntegrationSettings
{
    /// <summary>
    /// Gets or sets the base URL for the HTTP client.
    /// This property is required and must be a valid URL format.
    /// </summary>
    /// <value>
    /// The base address <see cref="Uri"/> of the internet resource used when sending requests.
    /// </value>
    Uri BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a proxy should be used for requests.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the handler should use a proxy for requests; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    bool UseProxy { get; set; }

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// The value must be between 1 and 600 seconds. Defaults to 100.
    /// </summary>
    /// <value>
    /// The number of seconds to wait for a response. The default is 100.
    /// </value>
    /// <remarks>
    /// For more information, see the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-8.0">HttpClient.Timeout documentation</see>.
    /// </remarks>
    double Timeout { get; set; }

    /// <summary>
    /// Gets or sets the media type for the content.
    /// </summary>
    /// <value>
    /// A string representing the media type, such as "application/json".
    /// </value>
    /// <remarks>
    /// This is used to set the <c>Content-Type</c> header for requests that have a body.
    /// </remarks>
    string MediaType { get; set; }

    /// <summary>
    /// Gets or sets the encoding for the content.
    /// Defaults to "utf-8".
    /// </summary>
    /// <value>A string representing the character encoding, such as "utf-8".</value>
    /// <remarks>
    /// This is used in conjunction with <see cref="RA.Utilities.Integrations.Abstractions.IIntegrationSettings.MediaType"/> to form the <c>Content-Type</c> header (e.g., "application/json; charset=utf-8").
    /// </remarks>
    string Encoding { get; set; }
}
