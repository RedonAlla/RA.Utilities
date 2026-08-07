using System.Linq;
using FluentAssertions;
using RA.Utilities.OpenApi.Settings;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.Settings;

/// <summary>
/// Tests for the <see cref="HeadersParameterSettings"/> class.
/// </summary>
public class HeadersParameterSettingsTests
{
    /// <summary>
    /// Tests that the default settings for request headers include "x-request-id".
    /// </summary>
    [Fact]
    public void DefaultSettings_ShouldContainXRequestIdInRequestHeaders()
    {
        var settings = new HeadersParameterSettings();

        settings.RequestHeaders.Should().ContainSingle(h => h.Name == "x-request-id");
    }

    /// <summary>
    /// Tests that the default "x-request-id" header in request headers is required and has a "uuid" format.
    /// </summary>
    [Fact]
    public void DefaultSettings_XRequestId_ShouldBeRequired()
    {
        var settings = new HeadersParameterSettings();

        HeaderDefinition header = settings.RequestHeaders.Single(h => h.Name == "x-request-id");
        header.Required.Should().BeTrue();
        header.Format.Should().Be("uuid");
    }

    /// <summary>
    /// Tests that the default settings for response headers include "x-request-id" and "trace-id".
    /// </summary>
    [Fact]
    public void DefaultSettings_ShouldContainXRequestIdAndTraceIdInResponseHeaders()
    {
        var settings = new HeadersParameterSettings();

        settings.ResponseHeaders.Should().Contain(h => h.Name == "x-request-id");
        settings.ResponseHeaders.Should().Contain(h => h.Name == "trace-id");
    }

    /// <summary>
    /// Tests that the example values for headers are deterministic across multiple instantiations.
    /// </summary>
    [Fact]
    public void DefaultSettings_ExampleValues_ShouldBeDeterministic()
    {
        var settings1 = new HeadersParameterSettings();
        var settings2 = new HeadersParameterSettings();

        string value1 = settings1.RequestHeaders[0].Value!.ToString();
        string value2 = settings2.RequestHeaders[0].Value!.ToString();

        value1.Should().Be(value2);
    }

    /// <summary>
    /// Tests that the <see cref="HeadersParameterSettings.AppSettingsKey"/> constant has the correct value.
    /// </summary>
    [Fact]
    public void AppSettingsKey_ShouldBeCorrect()
    {
        HeadersParameterSettings.AppSettingsKey.Should().Be("OpenApiHeaders");
    }
}
