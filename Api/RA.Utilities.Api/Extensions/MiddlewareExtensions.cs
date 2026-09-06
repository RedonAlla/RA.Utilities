using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IO;
using RA.Utilities.Api.ExceptionHandlers;
using RA.Utilities.Api.Middlewares;
using RA.Utilities.Api.Options;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides the <c>Add</c>/<c>Use</c> extension method pairs for registering and configuring all
/// middleware shipped with this package: the <see cref="DefaultHeadersMiddleware"/>, the
/// <see cref="GlobalExceptionHandler"/>, the <see cref="LoggingMiddleware"/>, and the
/// <see cref="RequestContextLoggingMiddleware"/>.
/// </summary>
public static class MiddlewareExtensions
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

    /// <summary>
    /// Registers the <see cref="GlobalExceptionHandler"/> as the application's global exception handler.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the handler to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRaExceptionHandling(this IServiceCollection services) =>
        services.AddExceptionHandler<GlobalExceptionHandler>();

    /// <summary>
    /// Adds the global exception handler middleware to the request pipeline.
    /// This should be called early in the pipeline to catch exceptions from
    /// subsequent middleware and endpoints.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseRaExceptionHandling(this IApplicationBuilder app) =>
        app.UseExceptionHandler();

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

    /// <summary>
    /// Adds the <see cref="RequestContextLoggingMiddleware"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRequestContextLoggingMiddleware(this IServiceCollection services) =>
        services.AddTransient<RequestContextLoggingMiddleware>();

    /// <summary>
    /// Registers the <see cref="RequestContextLoggingMiddleware"/> in the request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
    public static IApplicationBuilder UseRequestContextLoggingMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RequestContextLoggingMiddleware>();
}
