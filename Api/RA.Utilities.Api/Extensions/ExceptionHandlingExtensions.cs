using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.ExceptionHandlers;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for registering the <see cref="GlobalExceptionHandler"/>
/// with the ASP.NET Core dependency injection container and request pipeline.
/// </summary>
public static class ExceptionHandlingExtensions
{
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
}
