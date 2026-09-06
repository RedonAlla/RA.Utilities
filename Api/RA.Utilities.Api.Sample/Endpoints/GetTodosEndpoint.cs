using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Api.Sample.Models;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// Lists all todos. Mapped into the "Todos" group, so the full route is <c>GET api/todos</c>.
/// </summary>
internal sealed class GetTodosEndpoint : IEndpoint
{
    public static string GroupName => "Todos";

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        Todo[] todos =
        [
            new Todo(1, "Define the route groups", false),
            new Todo(2, "Write an endpoint", false),
        ];

        group.MapGet("/", () => HttpResults.Ok(todos));
    }
}
