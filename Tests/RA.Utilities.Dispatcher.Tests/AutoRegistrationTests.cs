using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the zero-config registration: the source generator runs on this test assembly,
/// so <see cref="MediatorServiceCollectionExtensions.AddMediator(IServiceCollection)"/> alone must
/// register every handler and closed behavior implemented in the assembly.
/// </summary>
public class AutoRegistrationTests
{
    [Fact]
    public void AddMediator_AutoRegistersRequestHandlersFromThisAssembly()
    {
        using ServiceProvider provider = CreateProvider();

        IRequestHandler<PlainResponseRequest, string> responseHandler =
            provider.GetRequiredService<IRequestHandler<PlainResponseRequest, string>>();
        IRequestHandler<VoidPassthroughRequest> voidHandler =
            provider.GetRequiredService<IRequestHandler<VoidPassthroughRequest>>();

        responseHandler.Should().NotBeNull();
        voidHandler.Should().NotBeNull();
    }

    [Fact]
    public void AddMediator_AutoRegistersNotificationHandlersFromThisAssembly()
    {
        using ServiceProvider provider = CreateProvider();

        INotificationHandler<FanOutNotification> handler =
            provider.GetRequiredService<INotificationHandler<FanOutNotification>>();

        handler.Should().NotBeNull();
    }

    [Fact]
    public void AddMediator_AutoRegistersClosedPipelineBehaviors_UnderTheirInterface()
    {
        using ServiceProvider provider = CreateProvider();

        IPipelineBehavior<ClosedBehaviorRequest, string> behavior =
            provider.GetRequiredService<IPipelineBehavior<ClosedBehaviorRequest, string>>();

        behavior.Should().BeOfType<ClosedNoOpBehavior>();
    }

    [Fact]
    public void AddMediator_RegistersHandlersAsTheirConcreteType()
    {
        using ServiceProvider provider = CreateProvider();

        PlainResponseHandler concrete = provider.GetRequiredService<PlainResponseHandler>();

        concrete.Should().NotBeNull();
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        return services.BuildServiceProvider();
    }
}

public record ClosedBehaviorRequest : IRequest<string>;

public class ClosedBehaviorHandler : IRequestHandler<ClosedBehaviorRequest, string>
{
    public System.Threading.Tasks.ValueTask<RA.Utilities.Core.Results.Result<string>> HandleAsync(
        ClosedBehaviorRequest request, System.Threading.CancellationToken cancellationToken) =>
        System.Threading.Tasks.ValueTask.FromResult(RA.Utilities.Core.Results.Result.Success("ok"));
}

/// <summary>
/// Closed (non-generic) behavior — the source generator auto-registers it under its interface.
/// </summary>
public class ClosedNoOpBehavior : IPipelineBehavior<ClosedBehaviorRequest, string>
{
    public System.Threading.Tasks.ValueTask<RA.Utilities.Core.Results.Result<string>> HandleAsync(
        ClosedBehaviorRequest request,
        RequestHandlerDelegate<string> next,
        System.Threading.CancellationToken cancellationToken) =>
        next();
}
