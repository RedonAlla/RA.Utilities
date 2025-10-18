using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.Options;

/// <summary>
/// Defines the basic contract for HTTP client integration settings.
/// This ensures that any settings class used with the integration helpers
/// provides the necessary properties to configure an HttpClient.
/// </summary>
/// <typeparam name="T">The type of the class that holds the API action/endpoint names.</typeparam>
public abstract class BaseApiSettings<T> : IIntegrationSettings
{
    /// <summary>
    /// Gets or sets the base URL for the HTTP client.
    /// This property is required and must be a valid URL format.
    /// </summary>
    /// <value>
    /// The base address <see cref="Uri"/> of the internet resource used when sending requests.
    /// </value>
    [Required(ErrorMessage = "BaseUrl is required.")]
    [Url(ErrorMessage = "BaseUrl must be a valid URL.")]
    public required Uri BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the container for API action/endpoint names.
    /// </summary>
    /// <value>
    /// An object of type <typeparamref name="T"/> that contains the definitions for the API endpoints.
    /// </value>
    public required T Actions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a proxy should be used for requests.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the handler should use a proxy for requests; otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool UseProxy { get; set; }

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// The value must be between 1 and 600 seconds. Defaults to 200.
    /// </summary>
    /// <value>
    /// The number of seconds to wait for a response. The default is 200.
    /// </value>
    /// <remarks>
    /// For more information, see the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-8.0">HttpClient.Timeout documentation</see>.
    /// </remarks>
    [Range(1, 600, ErrorMessage = "TimeoutSeconds must be between 1 and 600.")]
    public double Timeout { get; set; } = 200;

    /// <inheritdoc/>
    public string MediaType { get; set; } = MediaTypeNames.Application.Json;

    /// <inheritdoc/>
    public string Encoding { get; set; } = "utf-8";
}
