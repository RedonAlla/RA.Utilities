using System;
using System.Text;
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
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _clockSkew;
    private readonly SymmetricSecurityKey? _issuerSigningKey;
    private readonly Action<JwtBearerOptions>? _configureOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureJwtBearerOptions"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration, used to retrieve the JWT signing key.</param>
    /// <param name="configureOptions">
    /// An optional callback invoked <em>after</em> all configuration binding and special conversions.
    /// Use this to override or extend <see cref="JwtBearerOptions"/> programmatically.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the configured issuer signing key is present but shorter than the minimum required length of 32 bytes (256 bits).
    /// </exception>
    public ConfigureJwtBearerOptions(IConfiguration configuration, Action<JwtBearerOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
        _configureOptions = configureOptions;

        double clockSkewInSeconds = configuration.GetValue<double?>(KeyConstants.ClockSkewInSecondsKey) ?? 300; // Default to 5 minutes
        _clockSkew = TimeSpan.FromSeconds(clockSkewInSeconds);

        _issuerSigningKey = BuildIssuerSigningKey(configuration.GetValue<string>(KeyConstants.IssuerSigningKeyStringKey));
    }

    /// <summary>
    /// Configures the specified <see cref="JwtBearerOptions"/> for a named scheme.
    /// </summary>
    /// <remarks>
    /// Configuration is applied in this order:
    /// <list type="number">
    ///   <item>Bind standard <see cref="JwtBearerOptions"/> from the <c>Authentication:Schemes:Bearer</c> section.</item>
    ///   <item>Apply special conversions for <c>ClockSkewInSeconds</c> and <c>IssuerSigningKeyString</c>.</item>
    ///   <item>Invoke the user-provided callback, allowing programmatic overrides of any config-driven values.</item>
    /// </list>
    /// </remarks>
    /// <param name="name">The name of the options instance to configure.</param>
    /// <param name="options">The <see cref="JwtBearerOptions"/> to configure.</param>
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            // Step 1: Bind standard JwtBearerOptions from configuration
            _configuration.GetSection(KeyConstants.JwtBearerOptionsKey).Bind(options);

            // Step 2: Apply special conversions for ClockSkew and IssuerSigningKey
            options.TokenValidationParameters ??= new TokenValidationParameters();
            options.TokenValidationParameters.ClockSkew = _clockSkew;

            if (_issuerSigningKey is not null)
            {
                options.TokenValidationParameters.IssuerSigningKey = _issuerSigningKey;
            }

            // Step 3: User callback runs LAST so consumer overrides always win
            _configureOptions?.Invoke(options);
        }
    }

    /// <summary>
    /// Configures the default <see cref="JwtBearerOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="JwtBearerOptions"/> to configure.</param>
    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    /// <summary>
    /// Builds a <see cref="SymmetricSecurityKey"/> from the provided key string.
    /// </summary>
    /// <param name="keyString">The issuer signing key string from configuration.</param>
    /// <returns>
    /// A <see cref="SymmetricSecurityKey"/>, or <see langword="null"/> if <paramref name="keyString"/>
    /// is <see langword="null"/> or whitespace.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <paramref name="keyString"/> is shorter than the minimum required length of 32 bytes.
    /// </exception>
    private static SymmetricSecurityKey? BuildIssuerSigningKey(string? keyString)
    {
        if (string.IsNullOrWhiteSpace(keyString))
        {
            return null;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(keyString);

        // A key length of at least 32 bytes (256 bits) is required for HMAC-SHA256
        const int minKeySizeInBytes = 32;
        if (bytes.Length < minKeySizeInBytes)
        {
            throw new InvalidOperationException(
                $"The configured issuer signing key must be at least {minKeySizeInBytes} bytes long.");
        }

        return new SymmetricSecurityKey(bytes);
    }
}
