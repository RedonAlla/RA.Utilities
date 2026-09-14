using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using Xunit;

namespace RA.Utilities.Tests.RA.Utilities.Feature;

/// <summary>
/// Verifies the end-to-end mediator wiring: the test assembly's own generated registration
/// initializer feeds the real <see cref="global::RA.Utilities.Feature.Generated.MediatorRegistrations"/>
/// queue, so <see cref="MediatorServiceCollectionExtensions.AddMediator(IServiceCollection)"/>
/// resolves <c>IMediator</c> to the generated implementation unless opted out.
/// </summary>
public class MediatorRegistrationTests
{
    [Fact]
    public void AddMediator_ResolvesGeneratedImplementation_ByDefault()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        using ServiceProvider provider = services.BuildServiceProvider();

        provider.GetRequiredService<IMediator>().GetType().Name.Should().Be("Mediator");
    }

    [Fact]
    public async Task AddMediator_GeneratedImplementation_DispatchesRequest()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        using ServiceProvider provider = services.BuildServiceProvider();

        IMediator mediator = provider.GetRequiredService<IMediator>();

        string response = await mediator.Send<RegistrationTestRequest, string>(new RegistrationTestRequest("ping"));

        response.Should().Be("pong:ping");
    }
}

public record RegistrationTestRequest(string Value) : IRequest<string>;

public class RegistrationTestHandler : IRequestHandler<RegistrationTestRequest, string>
{
    public Task<string> HandleAsync(RegistrationTestRequest request, CancellationToken cancellationToken) =>
        Task.FromResult($"pong:{request.Value}");
}
