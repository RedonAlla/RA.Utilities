using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using Xunit;

namespace RA.Utilities.Tests.RA.Utilities.Feature;

/// <summary>
/// Verifies the zero-config registration: the source generator runs on this test assembly,
/// so <see cref="MediatorServiceCollectionExtensions.AddMediator(IServiceCollection)"/> alone must
/// register every handler implemented in the assembly.
/// </summary>
public class AutoRegistrationTests
{
    [Fact]
    public void AddMediator_AutoRegistersRequestHandlersFromThisAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        ServiceProvider provider = services.BuildServiceProvider();

        IRequestHandler<NoContextRequest, string> responseHandler =
            provider.GetRequiredService<IRequestHandler<NoContextRequest, string>>();
        IRequestHandler<ContextAwareRequest, string> contextHandler =
            provider.GetRequiredService<IRequestHandler<ContextAwareRequest, string>>();

        responseHandler.Should().NotBeNull();
        contextHandler.Should().NotBeNull();
    }

    [Fact]
    public void AddMediator_AutoRegistersNotificationHandlersFromThisAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator();
        ServiceProvider provider = services.BuildServiceProvider();

        INotificationHandler<TestNotification> handler =
            provider.GetRequiredService<INotificationHandler<TestNotification>>();

        handler.Should().NotBeNull();
    }
}
