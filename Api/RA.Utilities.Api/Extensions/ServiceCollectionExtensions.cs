using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IO;
using RA.Utilities.Api.ExceptionHandlers;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides the <c>Add</c> extension method pairs for registering and configuring all
/// middleware shipped with this package: the <see cref="DefaultHeadersMiddleware"/>, the
/// <see cref="GlobalExceptionHandler"/>, the <see cref="LoggingMiddleware"/>, and the
/// <see cref="ContextLoggingMiddleware"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the <see cref="DefaultHeadersMiddleware"/> and its dependencies to the specified <see cref="IServiceCollection"/>.
        /// Call this during service configuration before <see cref="ApplicationBuilderExtensions.UseDefaultHeadersMiddleware"/>.
        /// </summary>
        /// <param name="configureOptions">An optional <see cref="Action{DefaultHeadersOptions}"/> to configure the default headers options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddDefaultHeadersMiddleware(Action<DefaultHeadersOptions>? configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services.AddTransient<DefaultHeadersMiddleware>();
        }

        /// <summary>
        /// Registers the <see cref="GlobalExceptionHandler"/> as the application's global exception handler.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddRaExceptionHandling() =>
            services.AddExceptionHandler<GlobalExceptionHandler>();

        /// <summary>
        /// Adds the <see cref="LoggingMiddleware"/> and its dependencies to the specified <see cref="IServiceCollection"/>.
        /// Call this during service configuration before <see cref="ApplicationBuilderExtensions.UseLoggingMiddleware"/>.
        /// </summary>
        /// <param name="configureOptions">An optional <see cref="Action{HttpLoggingOptions}"/> to configure the logging options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddLoggingMiddleware(Action<HttpLoggingOptions>? configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            services.TryAddSingleton<RecyclableMemoryStreamManager>();
            return services.AddTransient<LoggingMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="ContextLoggingMiddleware"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddRequestContextLoggingMiddleware() =>
            services.AddTransient<ContextLoggingMiddleware>();
    }
}
