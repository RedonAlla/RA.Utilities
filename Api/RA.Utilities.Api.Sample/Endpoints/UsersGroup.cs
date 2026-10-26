using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// The shared route group for all User endpoints: the <c>api/v{version:apiVersion}/users</c> prefix
/// and the "Users" tag. When the application passes an <see cref="ApiVersionSet"/> to
/// <c>MapEndpoints</c>, it is applied to the group so every endpoint mapped into it is versioned.
/// </summary>
internal sealed class UsersGroup : IEndpointGroup
{
    public static string GroupName => "Users";

    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app, ApiVersionSet? apiVersionSet = null)
    {
        RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}/users").WithTags("Users");
        return apiVersionSet is null ? group : group.WithApiVersionSet(apiVersionSet);
    }
}
