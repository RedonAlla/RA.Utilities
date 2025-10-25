using System;
using RA.Utilities.Api.Middlewares.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;

namespace RA.Utilities.Api.Middlewares.Extensions;

/// <summary>
/// Extension methods for setting up HTTP logging middleware in an ASP.NET Core application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="HttpLoggingMiddleware"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">An optional <see cref="Action{HttpLoggingOptions}"/> to configure the logging options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddHttpLoggingMiddleware(this IServiceCollection services, Action<HttpLoggingOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<RecyclableMemoryStreamManager>();
        return services
            .AddTransient<HttpLoggingMiddleware>();
    }

    /// <summary>
    /// Adds the <see cref="DefaultHeadersMiddleware"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the middleware to.</param>
    /// <param name="configureOptions">An optional <see cref="Action{DefaultHeadersOptions}"/> to configure the default headers options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultHeadersMiddleware(this IServiceCollection services, Action<DefaultHeadersOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddTransient<DefaultHeadersMiddleware>();
        return services;
    }
}
