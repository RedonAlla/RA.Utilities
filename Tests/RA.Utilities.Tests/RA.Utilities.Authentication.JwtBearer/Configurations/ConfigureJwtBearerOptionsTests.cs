using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RA.Utilities.Authentication.JwtBearer.Configurations;

namespace RA.Utilities.Tests.RA.Utilities.Authentication.JwtBearer.Configurations;

/// <summary>
/// Contains unit tests for the <see cref="ConfigureJwtBearerOptions"/> class.
/// </summary>
public class ConfigureJwtBearerOptionsTests
{
    private const string ValidKey32Bytes = "this-is-a-valid-key-32-bytes-long!!";
    private const string ShortKey = "too-short";

    private static IConfiguration CreateConfiguration(
        double? clockSkewInSeconds = null,
        string? issuerSigningKeyString = null)
    {
        var configData = new Dictionary<string, string?>();
        string baseKey = "Authentication:Schemes:Bearer:TokenValidationParameters";

        if (clockSkewInSeconds.HasValue)
        {
            configData[$"{baseKey}:ClockSkewInSeconds"] = clockSkewInSeconds.Value.ToString(
                System.Globalization.CultureInfo.InvariantCulture);
        }

        if (issuerSigningKeyString is not null)
        {
            configData[$"{baseKey}:IssuerSigningKeyString"] = issuerSigningKeyString;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    // =================================================================
    // Constructor — null guards
    // =================================================================

    [Fact]
    public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => _ = new ConfigureJwtBearerOptions(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    // =================================================================
    // Constructor — default values
    // =================================================================

    [Fact]
    public void Constructor_WithEmptyConfiguration_ShouldUseDefaultClockSkew()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);

        // Assert — verify via Configure method
        var options = new JwtBearerOptions();
        sut.Configure(options);
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(300));
    }

    [Fact]
    public void Constructor_WithEmptyConfiguration_ShouldNotSetIssuerSigningKey()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.IssuerSigningKey.Should().BeNull();
    }

    // =================================================================
    // Constructor — ClockSkew
    // =================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(300)]
    [InlineData(900)]
    public void Constructor_WithClockSkewInSeconds_ShouldSetCorrectTimeSpan(int seconds)
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: seconds);

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(seconds));
    }

    // =================================================================
    // Constructor — IssuerSigningKey validation
    // =================================================================

    [Fact]
    public void Constructor_WithValidKey_ShouldCreateSymmetricSecurityKey()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: ValidKey32Bytes);

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.IssuerSigningKey.Should().NotBeNull();
        options.TokenValidationParameters.IssuerSigningKey.Should().BeOfType<SymmetricSecurityKey>();
    }

    [Fact]
    public void Constructor_WithValidKey_ShouldSetCorrectKeyMaterial()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: ValidKey32Bytes);

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        sut.Configure(options);

        // Assert
        byte[] expectedBytes = Encoding.UTF8.GetBytes(ValidKey32Bytes);
        var key = options.TokenValidationParameters.IssuerSigningKey as SymmetricSecurityKey;
        key!.Key.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public void Constructor_WithKeyExactly32Bytes_ShouldSucceed()
    {
        // Arrange
        string key32Bytes = new string('k', 32);
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: key32Bytes);

        // Act
        Action act = () => _ = new ConfigureJwtBearerOptions(configuration);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithKeyShorterThan32Bytes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: ShortKey);

        // Act
        Action act = () => _ = new ConfigureJwtBearerOptions(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*32*bytes*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespaceKey_ShouldNotThrow(string? keyString)
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: keyString);

        // Act
        Action act = () => _ = new ConfigureJwtBearerOptions(configuration);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithNullOrWhitespaceKey_ShouldNotSetIssuerSigningKey()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: "   ");

        // Act
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.IssuerSigningKey.Should().BeNull();
    }

    // =================================================================
    // Configure — named vs unnamed
    // =================================================================

    [Fact]
    public void Configure_WithDefaultSchemeName_ShouldApplySettings()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 45);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure(JwtBearerDefaults.AuthenticationScheme, options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(45));
    }

    [Fact]
    public void Configure_WithNonMatchingSchemeName_ShouldNotApplySettings()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 45);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(10);

        // Act
        sut.Configure("AnotherScheme", options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void Configure_WithNullSchemeName_ShouldNotApplySettings()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 45);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(10);

        // Act
        sut.Configure(null, options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void Configure_Unnamed_ShouldDelegateToDefaultScheme()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 60);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(60));
    }

    // =================================================================
    // Configure — TokenValidationParameters handling
    // =================================================================

    [Fact]
    public void Configure_WithExistingTokenValidationParameters_ShouldPreserveOtherProperties()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 45);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions
        {
#pragma warning disable CA5404 // intentionally setting to false in test to verify preservation
            TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
            }
        };
#pragma warning restore CA5404

        // Act
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.ValidateIssuer.Should().BeFalse();
        options.TokenValidationParameters.ValidateAudience.Should().BeFalse();
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(45));
    }

    [Fact]
    public void Configure_WithNullTokenValidationParameters_ShouldCreateNewInstance()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions
        {
            TokenValidationParameters = null!
        };

        // Act
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.Should().NotBeNull();
    }

    // =================================================================
    // Configure — config binding
    // =================================================================

    [Fact]
    public void Configure_ShouldBindStandardJwtBearerOptions()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["Authentication:Schemes:Bearer:Authority"] = "https://auth.example.com",
            ["Authentication:Schemes:Bearer:Audience"] = "test-api",
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert
        options.Authority.Should().Be("https://auth.example.com");
        options.Audience.Should().Be("test-api");
    }

    // =================================================================
    // configureOptions callback behavior
    // =================================================================

    [Fact]
    public void Configure_WithCallback_ShouldInvokeCallback()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();
        bool callbackInvoked = false;
        var sut = new ConfigureJwtBearerOptions(configuration, _ => callbackInvoked = true);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert
        callbackInvoked.Should().BeTrue();
    }

    [Fact]
    public void Configure_WithCallback_ShouldRunCallbackLast()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(clockSkewInSeconds: 300);

        var sut = new ConfigureJwtBearerOptions(configuration, options =>
            options.TokenValidationParameters.ClockSkew = TimeSpan.Zero);

        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert — callback overrides config-driven value
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Configure_WithCallback_ShouldAllowOverridingIssuerSigningKey()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(issuerSigningKeyString: ValidKey32Bytes);
        var customKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("another-key-that-is-32-bytes-lon!!"));

        var sut = new ConfigureJwtBearerOptions(configuration, options =>
            options.TokenValidationParameters.IssuerSigningKey = customKey);

        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert — callback overrides config-driven key
        options.TokenValidationParameters.IssuerSigningKey.Should().BeSameAs(customKey);
    }

    [Fact]
    public void Configure_WithCallback_AndNonMatchingSchemeName_ShouldNotInvokeCallback()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();
        bool callbackInvoked = false;
        var sut = new ConfigureJwtBearerOptions(configuration, _ => callbackInvoked = true);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure("AnotherScheme", options);

        // Assert
        callbackInvoked.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullCallback_ShouldNotThrow()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration();

        // Act
        Action act = () => _ = new ConfigureJwtBearerOptions(configuration, configureOptions: null);

        // Assert
        act.Should().NotThrow();
    }

    // =================================================================
    // Combined scenarios
    // =================================================================

    [Fact]
    public void Configure_WithBothClockSkewAndKey_ShouldApplyBoth()
    {
        // Arrange
        IConfiguration configuration = CreateConfiguration(
            clockSkewInSeconds: 120,
            issuerSigningKeyString: ValidKey32Bytes);
        var sut = new ConfigureJwtBearerOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        sut.Configure(options);

        // Assert
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(120));
        options.TokenValidationParameters.IssuerSigningKey.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_RepeatedConstruction_ShouldProduceIndependentInstances()
    {
        // Arrange
        IConfiguration config1 = CreateConfiguration(clockSkewInSeconds: 30);
        IConfiguration config2 = CreateConfiguration(clockSkewInSeconds: 90);

        // Act
        var sut1 = new ConfigureJwtBearerOptions(config1);
        var sut2 = new ConfigureJwtBearerOptions(config2);
        var options1 = new JwtBearerOptions();
        var options2 = new JwtBearerOptions();
        sut1.Configure(options1);
        sut2.Configure(options2);

        // Assert
        options1.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(30));
        options2.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(90));
    }
}
