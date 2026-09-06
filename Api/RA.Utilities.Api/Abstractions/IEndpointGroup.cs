using Microsoft.AspNetCore.Routing;

namespace RA.Utilities.Api.Abstractions;

/// <summary>
/// Represents a route group that <see cref="IEndpoint"/> implementations map their routes into.
/// Implement this interface on a type whose static <see cref="MapGroup"/> method builds the shared
/// <see cref="RouteGroupBuilder"/> (prefix, tags, versioning, conventions, and so on) for one feature
/// of the API. The <c>MapEndpoints</c> source-generated registration pipeline discovers every
/// implementation at compile time and invokes <see cref="MapGroup"/> exactly once, before any
/// endpoint is mapped, storing the resulting builder under <see cref="GroupName"/>.
/// </summary>
public interface IEndpointGroup
{
    /// <summary>
    /// Gets the unique name of this group. Every <see cref="IEndpoint"/> with the same
    /// <see cref="IEndpoint.GroupName"/> is mapped into the <see cref="RouteGroupBuilder"/> returned by
    /// <see cref="MapGroup"/>. Group names must be unique across all <see cref="IEndpointGroup"/>
    /// implementations in the assembly; duplicates are reported as a compile-time error.
    /// </summary>
    static abstract string GroupName { get; }

    /// <summary>
    /// Creates the shared <see cref="RouteGroupBuilder"/> for this group, e.g. by calling
    /// <see cref="Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/>
    /// and applying tags, versioning, or conventions. Invoked once when the generated
    /// <c>MapEndpoints</c> registration method is called, before any endpoint is mapped.
    /// </summary>
    /// <param name="app">The application's endpoint route builder to create the group on.</param>
    /// <returns>The configured route group builder.</returns>
    static abstract RouteGroupBuilder MapGroup(IEndpointRouteBuilder app);
}
