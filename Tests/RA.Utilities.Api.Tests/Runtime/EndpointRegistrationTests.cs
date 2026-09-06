using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Extensions;

namespace RA.Utilities.Api.Tests.Runtime;

/// <summary>
/// Contains end-to-end tests for the generated <c>MapEndpoints</c> registration method, exercised
/// against the real generated code that the analyzer produces for the test project itself.
/// </summary>
public class EndpointRegistrationTests
{
    /// <summary>
    /// Verifies that <c>MapEndpoints</c> registers every fixture group and endpoint, with each group
    /// factory called exactly once and all endpoints mapped into the right route group.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldRegisterAllGroupsAndEndpoints()
    {
        // Arrange
        TestTodosGroup.MapGroupCallCount = 0;
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        app.MapEndpoints();

        // Assert
        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .ToList();

        IEnumerable<string> routePatterns = endpoints
            .Select(endpoint => ((RouteEndpoint)endpoint).RoutePattern.RawText ?? string.Empty);

        routePatterns.Should().Contain(pattern => pattern == "api/todos/");
        routePatterns.Should().Contain(pattern => pattern == "api/todos/{id:int}");
        routePatterns.Should().Contain(pattern => pattern == "api/users/");

        endpoints.Should().Contain(endpoint => endpoint.Metadata.GetMetadata<ITagsMetadata>()!.Tags.Contains("Todos"));
        endpoints.Should().Contain(endpoint => endpoint.Metadata.GetMetadata<ITagsMetadata>()!.Tags.Contains("Users"));

        // Each group is mapped exactly once, and all of its endpoints go into that one group.
        TestTodosGroup.MapGroupCallCount.Should().Be(1);
    }

    /// <summary>
    /// Verifies that <c>MapEndpoints</c> returns the same builder for fluent chaining.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldReturnSameBuilder()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        IEndpointRouteBuilder result = app.MapEndpoints();

        // Assert
        result.Should().BeSameAs(app);
    }

    /// <summary>
    /// Verifies that group factories run before any endpoint is mapped (the registry is populated
    /// first), by recording the call order inside an instrumented in-memory compilation.
    /// </summary>
    [Fact]
    public void MapEndpoints_ShouldMapAllGroupsBeforeAnyEndpoint()
    {
        // Arrange
        const string source = """
            using Microsoft.AspNetCore.Builder;

            namespace Sample;

            internal static class CallLog
            {
                public static readonly System.Collections.Generic.List<string> Calls = new System.Collections.Generic.List<string>();
            }

            internal sealed class LoggedGroup : RA.Utilities.Api.Abstractions.IEndpointGroup
            {
                public static string GroupName => "Logged";
                public static Microsoft.AspNetCore.Routing.RouteGroupBuilder MapGroup(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
                {
                    CallLog.Calls.Add("group");
                    return app.MapGroup("api/logged");
                }
            }

            internal sealed class GetLoggedEndpoint : RA.Utilities.Api.Abstractions.IEndpoint
            {
                public static string GroupName => "Logged";
                public static void MapEndpoint(Microsoft.AspNetCore.Routing.RouteGroupBuilder group) => CallLog.Calls.Add("endpoint");
            }
            """;

        Assembly assembly = Generators.GeneratorTestHost.CompileAndLoad(Generators.GeneratorTestHost.RuntimeSources, source);
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        InvokeMapEndpoints(assembly, app);

        // Assert
        var calls = (List<string>)assembly.GetType("Sample.CallLog")!.GetField("Calls")!.GetValue(null)!;
        calls.Should().Equal("group", "endpoint");
    }

    /// <summary>
    /// Verifies that an endpoint whose group name is computed at runtime (and therefore cannot be
    /// validated at compile time) resolves against the real group name when it matches.
    /// </summary>
    [Fact]
    public void MapEndpoints_ComputedMatchingGroupName_ShouldResolveAtRuntime()
    {
        // Arrange
        const string source = """
            using Microsoft.AspNetCore.Builder;

            namespace Sample;

            internal sealed class ComputedGroup : RA.Utilities.Api.Abstractions.IEndpointGroup
            {
                public static string GroupName => typeof(ComputedGroup).Name;
                public static Microsoft.AspNetCore.Routing.RouteGroupBuilder MapGroup(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app) => app.MapGroup("api/computed");
            }

            internal sealed class GetComputedEndpoint : RA.Utilities.Api.Abstractions.IEndpoint
            {
                public static string GroupName => typeof(ComputedGroup).Name;
                public static void MapEndpoint(Microsoft.AspNetCore.Routing.RouteGroupBuilder group) => group.MapGet("/", () => "ok");
            }
            """;

        Assembly assembly = Generators.GeneratorTestHost.CompileAndLoad(Generators.GeneratorTestHost.RuntimeSources, source);
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        InvokeMapEndpoints(assembly, app);

        // Assert
        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .ToList();
        endpoints.Select(endpoint => ((RouteEndpoint)endpoint).RoutePattern.RawText).Should().Contain("api/computed/");
    }

    /// <summary>
    /// Verifies that an endpoint whose group name is computed at runtime and matches no group fails
    /// at startup with an exception naming the endpoint type and the missing group key.
    /// </summary>
    [Fact]
    public void MapEndpoints_ComputedUnknownGroupName_ShouldThrowNamingEndpointAndGroup()
    {
        // Arrange
        const string source = """
            using Microsoft.AspNetCore.Builder;

            namespace Sample;

            internal sealed class GetOrphanEndpoint : RA.Utilities.Api.Abstractions.IEndpoint
            {
                public static string GroupName => typeof(GetOrphanEndpoint).Name;
                public static void MapEndpoint(Microsoft.AspNetCore.Routing.RouteGroupBuilder group) => group.MapGet("/", () => "ok");
            }
            """;

        Assembly assembly = Generators.GeneratorTestHost.CompileAndLoad(Generators.GeneratorTestHost.RuntimeSources, source);
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        Action act = () => InvokeMapEndpoints(assembly, app);

        // Assert
        act.Should().Throw<TargetInvocationException>()
            .WithInnerExceptionExactly<InvalidOperationException>()
            .WithMessage("*GetOrphanEndpoint*");
    }

    /// <summary>
    /// Verifies that two groups with the same computed (non-constant) group name fail at startup
    /// with a clear exception, since compile-time validation is impossible for computed names.
    /// </summary>
    [Fact]
    public void MapEndpoints_DuplicateComputedGroupNames_ShouldThrowAtStartup()
    {
        // Arrange
        const string source = """
            using Microsoft.AspNetCore.Builder;

            namespace Sample;

            internal sealed class FirstGroup : RA.Utilities.Api.Abstractions.IEndpointGroup
            {
                public static string GroupName => "Same";
                public static Microsoft.AspNetCore.Routing.RouteGroupBuilder MapGroup(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app) => app.MapGroup("api/first");
            }

            internal sealed class SecondGroup : RA.Utilities.Api.Abstractions.IEndpointGroup
            {
                public static string GroupName => "Same";
                public static Microsoft.AspNetCore.Routing.RouteGroupBuilder MapGroup(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app) => app.MapGroup("api/second");
            }
            """;

        Assembly assembly = Generators.GeneratorTestHost.CompileAndLoad(Generators.GeneratorTestHost.RuntimeSources, source);
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        Action act = () => InvokeMapEndpoints(assembly, app);

        // Assert
        act.Should().Throw<TargetInvocationException>()
            .WithInnerExceptionExactly<InvalidOperationException>()
            .WithMessage("*Same*");
    }

    /// <summary>
    /// Invokes the generated <c>MapEndpoints</c> method from a loaded in-memory compilation against
    /// a real <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="assembly">The assembly containing the generated registration method.</param>
    /// <param name="app">The application to map the endpoints onto.</param>
    private static void InvokeMapEndpoints(Assembly assembly, WebApplication app)
    {
        Type registrationType = assembly.GetType("RA.Utilities.Api.Extensions.HttpEndpointServiceCollectionExtensions", throwOnError: true)!;
        MethodInfo mapEndpoints = registrationType.GetMethod("MapEndpoints")!;
        mapEndpoints.Invoke(null, [app]);
    }
}
