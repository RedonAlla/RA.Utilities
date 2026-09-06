using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Api.Sample.Models;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// Creates a todo. Mapped into the "Todos" group, so the full route is <c>POST api/todos</c>.
/// </summary>
internal sealed class CreateTodoEndpoint : IEndpoint
{
    public static string GroupName => "Todos";

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", (Todo todo) => HttpResults.Created($"/api/todos/{todo.Id}", todo));
    }
}
