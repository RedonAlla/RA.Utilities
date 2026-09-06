using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

namespace RA.Utilities.Api.Tests.Runtime;

/// <summary>
/// Test fixture: a "Todos" group that counts how many times its <see cref="MapGroup"/> factory runs.
/// These types are discovered by the generator that runs on the test project's own compilation,
/// so <c>app.MapEndpoints()</c> in the tests exercises the real generated registration code.
/// </summary>
internal sealed class TestTodosGroup : IEndpointGroup
{
    /// <summary>
    /// Gets or sets the number of times <see cref="MapGroup"/> has been invoked.
    /// </summary>
    public static int MapGroupCallCount { get; set; }

    /// <inheritdoc/>
    public static string GroupName => "Todos";

    /// <inheritdoc/>
    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app)
    {
        MapGroupCallCount++;
        return app.MapGroup("api/todos").WithTags("Todos");
    }
}

/// <summary>
/// Test fixture: a "Users" group.
/// </summary>
internal sealed class TestUsersGroup : IEndpointGroup
{
    /// <inheritdoc/>
    public static string GroupName => "Users";

    /// <inheritdoc/>
    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app) => app.MapGroup("api/users").WithTags("Users");
}

/// <summary>
/// Test fixture: an endpoint mapped into the "Todos" group.
/// </summary>
internal sealed class GetTestTodosEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public static string GroupName => "Todos";

    /// <inheritdoc/>
    public static void MapEndpoint(RouteGroupBuilder group) => group.MapGet("/", () => "[]");
}

/// <summary>
/// Test fixture: another endpoint mapped into the "Todos" group.
/// </summary>
internal sealed class GetTestTodoByIdEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public static string GroupName => "Todos";

    /// <inheritdoc/>
    public static void MapEndpoint(RouteGroupBuilder group) => group.MapGet("/{id:int}", (int id) => $"todo-{id}");
}

/// <summary>
/// Test fixture: an endpoint mapped into the "Users" group.
/// </summary>
internal sealed class GetTestUsersEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public static string GroupName => "Users";

    /// <inheritdoc/>
    public static void MapEndpoint(RouteGroupBuilder group) => group.MapGet("/", () => "[]");
}
