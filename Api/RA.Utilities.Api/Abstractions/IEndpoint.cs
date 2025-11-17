using Microsoft.AspNetCore.Routing;

namespace RA.Utilities.Api.Abstractions;

/// <summary>
/// Represents a feature's endpoints that can be dynamically registered with the application.
/// Implement this interface to group related API endpoints and keep `Program.cs` clean.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the specific endpoint's routes to the application's endpoint route builder.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to which the endpoint will be mapped.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
