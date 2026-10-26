using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Sample.Endpoints;

/// <summary>
/// The shared route group for all Todo endpoints: the <c>api/v{version:apiVersion}/todos</c> prefix
/// and the "Todos" tag. When the application passes an <see cref="ApiVersionSet"/> to
/// <c>MapEndpoints</c>, it is applied to the group so every endpoint mapped into it is versioned.
/// Created exactly once by the generated registration pipeline.
/// </summary>
internal sealed class TodosGroup : IEndpointGroup
{
    public static string GroupName => "Todos";

    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app, ApiVersionSet? apiVersionSet = null)
    {
        RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}/todos").WithTags("Todos");
        return apiVersionSet is null ? group : group.WithApiVersionSet(apiVersionSet);
    }
}
