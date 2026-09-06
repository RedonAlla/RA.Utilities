using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Api.Sample.Models;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// Gets a single user by id. Mapped into the "Users" group, so the full route is
/// <c>GET api/users/{id:int}</c>.
/// </summary>
internal sealed class GetUserByIdEndpoint : IEndpoint
{
    public static string GroupName => "Users";

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", (int id) =>
        {
            User? user = id switch
            {
                1 => new User(1, "Ada"),
                2 => new User(2, "Grace"),
                _ => null,
            };

            return user is null ? HttpResults.NotFound() : HttpResults.Ok(user);
        });
    }
}
