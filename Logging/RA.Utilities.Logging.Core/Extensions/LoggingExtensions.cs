using System;
using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Logging.Core.Enrichers;
using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Provides extension methods for configuring Serilog logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds the <see cref="RequestIdEnricher"/> to the logger enrichment configuration to log a request ID.
    /// </summary>
    /// <param name="enrich">The logger enrichment configuration.</param>
    /// <returns>A <see cref="LoggerConfiguration"/> for further logger configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="enrich"/> is <c>null</c>.</exception>
    public static LoggerConfiguration WithRequestIdEnricher(this LoggerEnrichmentConfiguration enrich)
    {
        ArgumentNullException.ThrowIfNull(enrich);
        return enrich.With<RequestIdEnricher>();
    }

    /// <summary>
    /// Configures Serilog as the logging provider for the application with common configurations.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure.</param>
    /// <remarks>
    /// This extension method configures Serilog by:
    /// <list type="bullet">
    /// <item><description>Reading logging configuration from the application's <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.</description></item>
    /// <item><description>Enriching logs with detailed exception information.</description></item>
    /// <item><description>Enabling destructuring of System.Text.Json types.</description></item>
    /// </list>
    /// If the <see cref="RequestIdEnricher"/> is activated through the logging configuration (for example via <see cref="WithRequestIdEnricher"/>),
    /// register <see cref="IHttpContextAccessor"/> yourself, for example with <c>builder.Services.AddHttpContextAccessor()</c>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
    public static void AddLoggingWithConfiguration(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration)
                        .Enrich.WithExceptionDetails()
                        .Destructure.SystemTextJsonTypes()
        );
    }
}
