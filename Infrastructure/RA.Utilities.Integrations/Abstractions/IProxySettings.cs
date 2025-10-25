using System.ComponentModel.DataAnnotations;

namespace RA.Utilities.Integrations.Abstractions;

/// <summary>
/// Defines a contract for settings that include proxy configuration.
/// </summary>
public interface IProxySettings
{
    /// <summary>
    /// Gets or sets the URI of the proxy server.
    /// Example: "http://proxy.example.com:8888"
    /// </summary>
    [Url]
    string? Address { get; set; }

    /// <summary>
    /// Gets or sets the username for proxy authentication.
    /// </summary>
    string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for proxy authentication.
    /// </summary>
    string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to bypass the proxy for local addresses.
    /// Defaults to true.
    /// </summary>
    bool BypassProxyOnLocal { get; set; }
}
