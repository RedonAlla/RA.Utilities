using System;
using RA.Utilities.Logging.Core.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Provides extension methods for configuring Serilog logging.
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions
{
    extension(LoggerEnrichmentConfiguration enrich)
    {
        /// <summary>
        /// Adds the <see cref="RequestIdEnricher"/> to the logger enrichment configuration to log a request ID.
        /// </summary>
        /// <returns>A <see cref="LoggerConfiguration"/> for further logger configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enrich"/> is <c>null</c>.</exception>
        public LoggerConfiguration WithRequestIdEnricher()
        {
            ArgumentNullException.ThrowIfNull(enrich);
            return enrich.With<RequestIdEnricher>();
        }
    }
}
