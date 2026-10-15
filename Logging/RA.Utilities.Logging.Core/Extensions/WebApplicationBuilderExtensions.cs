using System;
using Destructurama;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WebApplicationBuilder"/> for Serilog logging.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Configures Serilog as the logging provider for the application with common configurations.
        /// </summary>
        /// <remarks>
        /// This extension method configures Serilog by:
        /// <list type="bullet">
        /// <item><description>Reading logging configuration from the application's <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.</description></item>
        /// <item><description>Enriching logs with detailed exception information.</description></item>
        /// <item><description>Enabling destructuring of System.Text.Json types.</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
        public void AddLoggingWithConfiguration()
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Host.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration)
                            .Enrich.WithExceptionDetails()
                            .Destructure.SystemTextJsonTypes()
            );
        }
    }
}
