using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Authorization;
using RA.Utilities.Authorization.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Authorization.Extensions;

public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddAppUser_ShouldRegisterAppUser()
    {
        var services = new ServiceCollection();
        services.AddAppUser();
        ServiceProvider provider = services.BuildServiceProvider();

        AppUser? user = provider.GetService<AppUser>();
        user.Should().NotBeNull();
    }

    [Fact]
    public void AddAppUser_ShouldReturnServiceCollection()
    {
        var services = new ServiceCollection();
        IServiceCollection result = services.AddAppUser();
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddAppUser_ShouldRegisterMultipleResolveInstances()
    {
        var services = new ServiceCollection();
        services.AddAppUser();
        ServiceProvider provider = services.BuildServiceProvider();

        AppUser user1 = provider.GetRequiredService<AppUser>();
        AppUser user2 = provider.GetRequiredService<AppUser>();

        // Transient — each resolve should produce a different instance
        user1.Should().NotBeSameAs(user2);
    }
}
