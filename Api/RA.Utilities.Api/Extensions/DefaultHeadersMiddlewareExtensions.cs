using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring the <see cref="DefaultHeadersMiddleware"/> in the application.
/// </summary>
public static class DefaultHeadersMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="DefaultHeadersMiddleware"/> and its dependencies to the specified <see cref="IServiceCollection"/>.
    /// Call this during service configuration before <see cref="UseDefaultHeadersMiddleware"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">An optional <see cref="Action{DefaultHeadersOptions}"/> to configure the default headers options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultHeadersMiddleware(this IServiceCollection services, Action<DefaultHeadersOptions>? configureOptions = null)
    {
        if (configureOptions != null)
            services.Configure(configureOptions);

        return services.AddTransient<DefaultHeadersMiddleware>();
    }

    /// <summary>
    /// Registers the <see cref="DefaultHeadersMiddleware"/> in the request pipeline.
    /// Must be called after <see cref="AddDefaultHeadersMiddleware"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
    public static IApplicationBuilder UseDefaultHeadersMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<DefaultHeadersMiddleware>();
}
