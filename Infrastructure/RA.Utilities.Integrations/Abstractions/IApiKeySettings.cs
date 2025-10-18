using System;

namespace RA.Utilities.Integrations.Abstractions;

/// <summary>
/// Defines a contract for settings that include an API key for authentication.
/// </summary>
public interface IApiKeySettings
{
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    string ApiKey { get; set; }
}
