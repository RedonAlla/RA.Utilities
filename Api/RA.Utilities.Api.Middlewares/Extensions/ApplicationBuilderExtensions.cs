using Microsoft.AspNetCore.Builder;

namespace RA.Utilities.Api.Middlewares.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to add custom middlewares.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Register HTTP request response middleware.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional Middlewares can be chained.</returns>
    public static IApplicationBuilder UseHttpLoggingMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<HttpLoggingMiddleware>();

    /// <summary>
    /// Registers the <see cref="DefaultHeadersMiddleware"/> to the specified <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional Middlewares can be chained.</returns>
    public static IApplicationBuilder UseDefaultHeadersMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<DefaultHeadersMiddleware>();
}
