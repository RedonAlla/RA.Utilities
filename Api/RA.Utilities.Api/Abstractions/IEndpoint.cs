using Microsoft.AspNetCore.Routing;

namespace RA.Utilities.Api.Abstractions;

/// <summary>
/// Defines the contract for an API endpoint that can be mapped by the application.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the specific endpoint's routes to the application's endpoint route builder.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to which the endpoint will be mapped.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
