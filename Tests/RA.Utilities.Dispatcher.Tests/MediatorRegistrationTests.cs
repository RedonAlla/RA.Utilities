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
/// Verifies the end-to-end mediator wiring: the test assembly's own generated registration
/// initializer feeds the real <c>RA.Utilities.Dispatcher.Generated.MediatorRegistrations</c>
/// queue, so <c>AddMediator()</c> resolves <c>IDispatcher</c> to the generated implementation
/// and the concrete <c>Mediator</c> can be injected directly.
/// </summary>
public class MediatorRegistrationTests
{
    [Fact]
    public void AddMediator_ResolvesGeneratedImplementation()
    {
        using ServiceProvider provider = CreateProvider();

        provider.GetRequiredService<IDispatcher>().GetType().Name.Should().Be("Mediator");
    }

    [Fact]
    public async Task AddMediator_GeneratedImplementation_DispatchesRequest()
    {
        using ServiceProvider provider = CreateProvider();

        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<RegistrationTestRequest, string>(
            new RegistrationTestRequest("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task ConcreteMediator_IsResolvable_AndDispatchesRequest()
    {
        using ServiceProvider provider = CreateProvider();

        Mediator mediator = provider.GetRequiredService<Mediator>();

        Result<string> result = await mediator.Send(new RegistrationTestRequest("ping"));

        result.Value.Should().Be("pong:ping");
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        return services.BuildServiceProvider();
    }
}

public record RegistrationTestRequest(string Value) : IRequest<string>;

public class RegistrationTestHandler : IRequestHandler<RegistrationTestRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(RegistrationTestRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success($"pong:{request.Value}"));
}
