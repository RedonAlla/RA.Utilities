using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.Middlewares;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring the <see cref="RequestContextLoggingMiddleware"/> in the application.
/// </summary>
public static class RequestContextLoggingMiddlewareExtensions
{
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
