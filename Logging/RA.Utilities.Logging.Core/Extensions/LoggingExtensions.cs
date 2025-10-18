using RA.Utilities.Logging.Core.Enrichers;
using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions;
using System;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Class containing extensions methods about logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds the <see cref="RequestIdEnricher"/> to the logger enrichment configuration to log a request ID.
    /// </summary>
    /// <param name="enrich">The logger enrichment configuration.</param>
    /// <returns>A <see cref="LoggerConfiguration"/> for further logger configuration.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="enrich"/> is <c>null</c>.</exception>
    public static LoggerConfiguration WithRequestIdEnricher(this LoggerEnrichmentConfiguration enrich)
    {
        ArgumentNullException.ThrowIfNull(enrich);
        return enrich.With<RequestIdEnricher>();
    }

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Configures Serilog as the logging provider for the application with common configurations.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure.</param>
    /// <remarks>
    /// This extension method configures Serilog by:
    /// <list type="bullet">
    /// <item><description>Reading logging configuration from the application's <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.</description></item>
    /// <item><description>Enriching logs with a request ID using <see cref="WithRequestIdEnricher"/>.</description></item>
    /// <item><description>Enriching logs with detailed exception information using <see cref="Serilog.Exceptions.ExceptionEnricherExtensions.WithExceptionDetails(LoggerEnrichmentConfiguration)"/>.</description></item>
    /// <item><description>Enabling destructuring of JSON.NET types using <see cref="Destructurama.LoggerConfigurationJsonNetExtensions.JsonNetTypes(Serilog.Configuration.LoggerDestructuringConfiguration)"/>.</description></item>
    /// <item><description>Ensuring <see cref="IHttpContextAccessor"/> is registered as a singleton service, which is required by <see cref="RequestIdEnricher"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
    public static void AddLoggingWithConfiguration(this WebApplicationBuilder builder)
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration)
                        .Enrich.WithRequestIdEnricher()
                        .Enrich.WithExceptionDetails()
                        .Destructure.SystemTextJsonTypes()
        );

        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}
