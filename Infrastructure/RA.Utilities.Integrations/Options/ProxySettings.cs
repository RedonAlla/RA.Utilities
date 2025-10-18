using System;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.Options;

/// <summary>
/// A concrete implementation of <see cref="IProxySettings"/> for configuration binding.
/// </summary>
public class ProxySettings : IProxySettings
{
    /// <summary>
    /// Gets or sets the address of the proxy server.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the username for proxy authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for proxy authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to bypass the proxy server for local addresses.
    /// </summary>
    public bool BypassProxyOnLocal { get; set; }
}
