using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IO;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring the <see cref="LoggingMiddleware"/> in the application.
/// </summary>
public static class LoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="LoggingMiddleware"/> and its dependencies to the specified <see cref="IServiceCollection"/>.
    /// Call this during service configuration before <see cref="UseLoggingMiddleware"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">An optional <see cref="Action{HttpLoggingOptions}"/> to configure the logging options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddLoggingMiddleware(this IServiceCollection services, Action<HttpLoggingOptions>? configureOptions = null)
    {
        if (configureOptions != null)
            services.Configure(configureOptions);

        services.TryAddSingleton<RecyclableMemoryStreamManager>();
        return services.AddTransient<LoggingMiddleware>();
    }

    /// <summary>
    /// Registers the <see cref="LoggingMiddleware"/> in the request pipeline.
    /// Must be called after <see cref="AddLoggingMiddleware"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<LoggingMiddleware>();
}
