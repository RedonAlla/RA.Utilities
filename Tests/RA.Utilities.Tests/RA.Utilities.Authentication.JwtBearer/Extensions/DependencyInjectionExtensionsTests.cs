using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RA.Utilities.Authentication.JwtBearer.Configurations;
using RA.Utilities.Authentication.JwtBearer.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Authentication.JwtBearer.Extensions;

/// <summary>
/// Contains unit tests for the <see cref="DependencyInjectionExtensions"/> class.
/// </summary>
public class DependencyInjectionExtensionsTests
{
    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();
    }

    // =================================================================
    // AddJwtBearerAuthentication — null guards
    // =================================================================

    [Fact]
    public void AddJwtBearerAuthentication_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Action act = () => services.AddJwtBearerAuthentication(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    // =================================================================
    // AddJwtBearerAuthentication — service registration
    // =================================================================

    [Fact]
    public void AddJwtBearerAuthentication_ShouldRegisterConfigureJwtBearerOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();

        // Act
        services.AddJwtBearerAuthentication(configuration);
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert — resolve all IConfigureOptions<JwtBearerOptions>
        IEnumerable<IConfigureOptions<JwtBearerOptions>> configureOptions =
            provider.GetServices<IConfigureOptions<JwtBearerOptions>>();

        configureOptions.Should().ContainSingle(o => o is ConfigureJwtBearerOptions);
    }

    [Fact]
    public void AddJwtBearerAuthentication_ShouldSetDefaultAuthenticationScheme()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();

        // Act
        services.AddJwtBearerAuthentication(configuration);
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        AuthenticationOptions authOptions =
            provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        authOptions.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authOptions.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authOptions.DefaultScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
    }

    [Fact]
    public void AddJwtBearerAuthentication_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();

        // Act
        IServiceCollection result = services.AddJwtBearerAuthentication(configuration);

        // Assert
        result.Should().BeSameAs(services);
    }

    // =================================================================
    // AddJwtBearerAuthentication — configureOptions callback
    // =================================================================

    [Fact]
    public void AddJwtBearerAuthentication_WithNullCallback_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();

        // Act
        Action act = () => services.AddJwtBearerAuthentication(configuration, configureOptions: null);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddJwtBearerAuthentication_WithCallback_ShouldPassCallbackThrough()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();
        bool callbackCaptured = false;
        Action<JwtBearerOptions> callback = _ => callbackCaptured = true;

        // Act
        services.AddJwtBearerAuthentication(configuration, callback);
        ServiceProvider provider = services.BuildServiceProvider();

        IEnumerable<IConfigureOptions<JwtBearerOptions>> configureOptions =
            provider.GetServices<IConfigureOptions<JwtBearerOptions>>();

        ConfigureJwtBearerOptions jwtConfig =
            configureOptions.OfType<ConfigureJwtBearerOptions>().Single();

        // Invoke to verify callback is wired
        var options = new JwtBearerOptions();
        jwtConfig.Configure(options);

        // Assert
        callbackCaptured.Should().BeTrue();
    }

    // =================================================================
    // AddJwtBearerAuthentication — idempotency
    // =================================================================

    [Fact]
    public void AddJwtBearerAuthentication_CalledTwice_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();

        // Act
        services.AddJwtBearerAuthentication(configuration);
        Action act = () => services.AddJwtBearerAuthentication(configuration);

        // Assert — AddAuthorization and AddAuthentication use TryAdd internally
        act.Should().NotThrow();
    }

    // =================================================================
    // UseAuth — middleware registration
    // =================================================================

    [Fact]
    public void UseAuth_ShouldReturnApplicationBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();
        services.AddJwtBearerAuthentication(configuration);
        ServiceProvider provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        // Act
        IApplicationBuilder result = app.UseAuth();

        // Assert
        result.Should().BeSameAs(app);
    }

    [Fact]
    public void UseAuth_ShouldNotThrowWithValidServices()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = CreateConfiguration();
        services.AddJwtBearerAuthentication(configuration);
        ServiceProvider provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        // Act
        Action act = () => app.UseAuth();

        // Assert
        act.Should().NotThrow();
    }
}
