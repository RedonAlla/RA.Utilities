using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.EndpointFilters;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="RouteHandlerBuilder"/> to add validation filters.
/// </summary>
public static class EndpointBuilderExtensions
{
    /// <summary>
    /// Adds an endpoint filter that automatically validates a model of type <typeparamref name="TModel"/> using FluentValidation.
    /// If validation fails, a validation error is returned before the Minimal API handler is executed.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to validate.</typeparam>
    /// <param name="builder">The route handler builder (e.g. MapGet, MapGroup, etc.).</param>
    /// <returns>The route handler builder with the validation filter applied.</returns>
    public static RouteHandlerBuilder Validate<TModel>(this RouteHandlerBuilder builder)
        where TModel : class => builder.AddEndpointFilter<ValidationEndpointFilter<TModel>>();
}
