using System;

namespace RA.Utilities.Authentication.JwtBearer.Constants;

/// <summary>
/// Contains constants for configuration keys related to JWT Bearer authentication options.
/// </summary>
internal static class KeyConstants
{
    /// <summary>
    /// The configuration key for JWT Bearer options.
    /// </summary>
    public const string JwtBearerOptionsKey = "Authentication:Schemes:Bearer";

    /// <summary>
    /// The configuration key for the clock skew in seconds for token validation.
    /// </summary>
    public const string ClockSkewInSecondsKey = JwtBearerOptionsKey + ":TokenValidationParameters:ClockSkewInSeconds";

    /// <summary>
    /// The configuration key for the issuer signing key string.
    /// </summary>
    public const string IssuerSigningKeyStringKey = JwtBearerOptionsKey + ":TokenValidationParameters:IssuerSigningKeyString";
}
