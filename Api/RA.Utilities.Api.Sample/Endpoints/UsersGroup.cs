using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// The shared route group for all User endpoints: the <c>api/users</c> prefix and the "Users" tag.
/// </summary>
internal sealed class UsersGroup : IEndpointGroup
{
    public static string GroupName => "Users";

    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app) =>
        app.MapGroup("api/users").WithTags("Users");
}
