using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Api.Sample.Models;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// Lists all users. Mapped into the "Users" group, so the full route is <c>GET api/users</c>.
/// </summary>
internal sealed class GetUsersEndpoint : IEndpoint
{
    public static string GroupName => "Users";

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        User[] users =
        [
            new User(1, "Ada"),
            new User(2, "Grace"),
        ];

        group.MapGet("/", () => HttpResults.Ok(users));
    }
}
