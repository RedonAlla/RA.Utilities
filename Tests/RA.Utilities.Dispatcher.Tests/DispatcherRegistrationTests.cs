using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Generated;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the end-to-end dispatcher wiring: the test assembly's own generated registration
/// initializer feeds the real <c>RA.Utilities.Dispatcher.Generated.DispatcherRegistrations</c>
/// queue, so <c>AddDispatcher()</c> resolves <c>IDispatcher</c> to the generated implementation
/// and the concrete <c>Dispatcher</c> can be injected directly.
/// </summary>
public class DispatcherRegistrationTests
{
    [Fact]
    public void AddDispatcher_ResolvesGeneratedImplementation()
    {
        using ServiceProvider provider = CreateProvider();

        provider.GetRequiredService<IDispatcher>().GetType().Name.Should().Be("Dispatcher");
    }

    [Fact]
    public async Task AddDispatcher_GeneratedImplementation_DispatchesRequest()
    {
        using ServiceProvider provider = CreateProvider();

        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<RegistrationTestRequest, string>(
            new RegistrationTestRequest("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task ConcreteDispatcher_IsResolvable_AndDispatchesRequest()
    {
        using ServiceProvider provider = CreateProvider();

        RA.Utilities.Dispatcher.Generated.Dispatcher dispatcher =
            provider.GetRequiredService<RA.Utilities.Dispatcher.Generated.Dispatcher>();

        Result<string> result = await dispatcher.Send(new RegistrationTestRequest("ping"));

        result.Value.Should().Be("pong:ping");
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher();
        return services.BuildServiceProvider();
    }
}

public record RegistrationTestRequest(string Value) : IRequest<string>;

public class RegistrationTestHandler : IRequestHandler<RegistrationTestRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(RegistrationTestRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success($"pong:{request.Value}"));
}
