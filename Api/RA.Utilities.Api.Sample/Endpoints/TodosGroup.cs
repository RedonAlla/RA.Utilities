using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// The shared route group for all Todo endpoints: the <c>api/todos</c> prefix and the "Todos" tag.
/// Created exactly once by the generated registration pipeline.
/// </summary>
internal sealed class TodosGroup : IEndpointGroup
{
    public static string GroupName => "Todos";

    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app) =>
        app.MapGroup("api/todos").WithTags("Todos");
}
