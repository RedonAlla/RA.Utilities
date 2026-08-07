using System;
using FluentAssertions;
using RA.Utilities.OpenApi.Settings;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.Settings;

/// <summary>
/// Tests for the <see cref="OpenApiInfoSettings"/> class.
/// </summary>
public class OpenApiInfoSettingsTests
{
    /// <summary>
    /// Tests that the default settings for <see cref="OpenApiInfoSettings.UiReferenceEndpoint"/> is "/".
    /// </summary>
    [Fact]
    public void DefaultSettings_ShouldHaveDefaultUiReferenceEndpoint()
    {
        var settings = new OpenApiInfoSettings();

        settings.UiReferenceEndpoint.Should().Be("/");
    }

    /// <summary>
    /// Tests that the default settings for <see cref="OpenApiInfoSettings.Title"/> is null.
    /// </summary>
    [Fact]
    public void DefaultSettings_Title_ShouldBeNull()
    {
        var settings = new OpenApiInfoSettings();
        settings.Title.Should().BeNull();
    }

    /// <summary>
    /// Tests that the default settings for <see cref="OpenApiInfoSettings.Version"/> is null.
    /// </summary>
    [Fact]
    public void DefaultSettings_Version_ShouldBeNull()
    {
        var settings = new OpenApiInfoSettings();
        settings.Version.Should().BeNull();
    }

    /// <summary>
    /// Tests that the <see cref="OpenApiInfoSettings.AppSettingsKey"/> constant has the correct value.
    /// </summary>
    [Fact]
    public void AppSettingsKey_ShouldBeCorrect()
    {
        OpenApiInfoSettings.AppSettingsKey.Should().Be("OpenApiInfoSettings");
    }

    /// <summary>
    /// Tests that contact settings can be set correctly.
    /// </summary>
    [Fact]
    public void CanSetContactSettings()
    {
        var settings = new OpenApiInfoSettings
        {
            Contact = new OpenApiContactSettings
            {
                Name = "Test Contact",
                Email = "test@example.com",
                Url = new Uri("https://example.com")
            }
        };

        settings.Contact.Should().NotBeNull();
        settings.Contact.Name.Should().Be("Test Contact");
        settings.Contact.Email.Should().Be("test@example.com");
        settings.Contact.Url.Should().Be(new Uri("https://example.com"));
    }

    /// <summary>
    /// Tests that license settings can be set correctly.
    /// </summary>
    [Fact]
    public void CanSetLicenseSettings()
    {
        var settings = new OpenApiInfoSettings
        {
            License = new OpenApiLicenseSettings
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        };

        settings.License.Should().NotBeNull();
        settings.License.Name.Should().Be("MIT");
        settings.License.Url.Should().Be(new Uri("https://opensource.org/licenses/MIT"));
    }
}
