using Microsoft.AspNetCore.Routing;

namespace RA.Utilities.Api.Abstractions;

/// <summary>
/// Represents a single endpoint that belongs to an <see cref="IEndpointGroup"/>.
/// Implement this interface on a type whose static <see cref="MapEndpoint"/> method defines one or more
/// routes. The <c>MapEndpoints</c> source-generated registration pipeline discovers every
/// implementation at compile time and invokes <see cref="MapEndpoint"/> with the
/// <see cref="RouteGroupBuilder"/> created by the <see cref="IEndpointGroup"/> whose
/// <see cref="IEndpointGroup.GroupName"/> matches <see cref="GroupName"/>.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Gets the name of the <see cref="IEndpointGroup"/> this endpoint belongs to.
    /// The source generator matches this value against the <see cref="IEndpointGroup.GroupName"/>
    /// of every discovered group at compile time.
    /// </summary>
    static abstract string GroupName { get; }

    /// <summary>
    /// Maps the endpoint's routes to the <see cref="RouteGroupBuilder"/> of the group identified by
    /// <see cref="GroupName"/>. Invoked once per endpoint when the generated <c>MapEndpoints</c>
    /// registration method is called.
    /// </summary>
    /// <param name="group">The route group this endpoint's routes are mapped into.</param>
    static abstract void MapEndpoint(RouteGroupBuilder group);
}
