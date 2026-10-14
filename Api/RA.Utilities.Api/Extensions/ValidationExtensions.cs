using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.EndpointFilters;

namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides extension methods for adding validation to Minimal API endpoints.
/// </summary>
public static class ValidationExtensions
{
    extension(RouteHandlerBuilder builder)
    {
        /// <summary>
        /// Adds an endpoint filter that automatically validates a model of type <typeparamref name="TModel"/> using FluentValidation.
        /// If validation fails, a validation error is returned before the Minimal API handler is executed.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to validate.</typeparam>
        /// <returns>The route handler builder with the validation filter applied.</returns>
        public RouteHandlerBuilder Validate<TModel>()
            where TModel : class => builder.AddEndpointFilter<ValidationEndpointFilter<TModel>>();
    }
}
