using System;
using System.Text;
using RA.Utilities.Authentication.JwtBearer.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace RA.Utilities.Authentication.JwtBearer.Configurations;

/// <summary>
/// Configures <see cref="JwtBearerOptions"/> by binding to the application's configuration
/// and setting the issuer signing key from a string value.
/// </summary>
/// <remarks>
/// This class is registered as an <see cref="IConfigureNamedOptions{JwtBearerOptions}"/>
/// to automatically apply settings from <c>appsettings.json</c> and handle the conversion
/// of the <c>IssuerSigningKeyString</c> into a <see cref="SymmetricSecurityKey"/>.
/// </remarks>
public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly TimeSpan _clockSkew;
    private readonly string? _issuerSigningKeyString;

    /// <summary>
    /// Configures the specified <see cref="JwtBearerOptions"/> instance by binding it to the
    /// configuration section and setting the issuer signing key.
    /// </summary>
    /// <param name="configuration">The application configuration, used to retrieve the JWT signing key.</param>
    public ConfigureJwtBearerOptions(IConfiguration configuration)
    {
        double clockSkewInSeconds = configuration.GetValue<double?>(KeyConstants.ClockSkewInSecondsKey) ?? 300; // Default to 5 minutes
        _clockSkew = TimeSpan.FromSeconds(clockSkewInSeconds);

        _issuerSigningKeyString = configuration.GetValue<string>(KeyConstants.IssuerSigningKeyStringKey);
    }

    /// <summary>
    /// Configures the specified <see cref="JwtBearerOptions"/> for a named scheme.
    /// </summary>
    /// <param name="name">The name of the options instance to configure.</param>
    /// <param name="options">The <see cref="JwtBearerOptions"/> to configure.</param>
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            options.TokenValidationParameters ??= new TokenValidationParameters();
            options.TokenValidationParameters.ClockSkew = _clockSkew;

            if (IssuerSigningKeyValue != null)
            {
                options.TokenValidationParameters.IssuerSigningKey = IssuerSigningKeyValue;
            }
        }
    }

    /// <summary>
    /// Configures the default <see cref="JwtBearerOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="JwtBearerOptions"/> to configure.</param>
    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    /// <summary>
    /// Gets the symmetric security key for signing the JWT.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the issuer signing key is not configured.</exception>
    private SymmetricSecurityKey? IssuerSigningKeyValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_issuerSigningKeyString))
            {
                return null;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(_issuerSigningKeyString!);

            // A key length of at least 32 bytes (256 bits) is recommended for HMAC-SHA256
            const int minKeySizeInBytes = 32;
            if (bytes.Length < minKeySizeInBytes)
            {
                // Or log a warning, depending on desired strictness. Throwing is safer.
                throw new InvalidOperationException($"The configured issuer signing key must be at least {minKeySizeInBytes} bytes long.");
            }

            return new SymmetricSecurityKey(bytes);
        }
    }
}
