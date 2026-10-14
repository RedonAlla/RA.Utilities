using Microsoft.AspNetCore.Builder;
using RA.Utilities.Api.ExceptionHandlers;
using RA.Utilities.Api.Middlewares;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides the <c>Use</c> extension method pairs for registering and configuring all
/// middleware shipped with this package: the <see cref="DefaultHeadersMiddleware"/>, the
/// <see cref="GlobalExceptionHandler"/>, the <see cref="LoggingMiddleware"/>, and the
/// <see cref="ContextLoggingMiddleware"/>.
/// </summary>
public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        /// <summary>
        /// Registers the <see cref="DefaultHeadersMiddleware"/> in the request pipeline.
        /// Must be called after <see cref="ServiceCollectionExtensions.AddDefaultHeadersMiddleware"/>.
        /// </summary>
        /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
        public IApplicationBuilder UseDefaultHeadersMiddleware() =>
            app.UseMiddleware<DefaultHeadersMiddleware>();

        /// <summary>
        /// Adds the global exception handler middleware to the request pipeline.
        /// This should be called early in the pipeline to catch exceptions from
        /// subsequent middleware and endpoints.
        /// </summary>
        /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
        public IApplicationBuilder UseRaExceptionHandling() =>
            app.UseExceptionHandler();

        /// <summary>
        /// Registers the <see cref="LoggingMiddleware"/> in the request pipeline.
        /// Must be called after <see cref="ServiceCollectionExtensions.AddLoggingMiddleware"/>.
        /// </summary>
        /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
        public IApplicationBuilder UseLoggingMiddleware() =>
            app.UseMiddleware<LoggingMiddleware>();

        /// <summary>
        /// Registers the <see cref="ContextLoggingMiddleware"/> in the request pipeline.
        /// </summary>
        /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
        public IApplicationBuilder UseContextLoggingMiddleware() =>
            app.UseMiddleware<ContextLoggingMiddleware>();
    }
}
