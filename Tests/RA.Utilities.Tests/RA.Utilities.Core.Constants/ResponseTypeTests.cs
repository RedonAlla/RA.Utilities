using System.Linq;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Tests.RA.Utilities.Core.Constants;

/// <summary>
/// Contains unit tests for the <see cref="ResponseType"/> enum,
/// including member values and serialization.
/// </summary>
public class ResponseTypeTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    // =================================================================
    // Built-in values
    // =================================================================

    /// <summary>
    /// Provides all built-in ResponseType members and their expected string values.
    /// </summary>
    public static TheoryData<ResponseType, string> BuiltInValues =>
        new()
        {
            { ResponseType.Success, "Success" },
            { ResponseType.Created, "Created" },
            { ResponseType.Updated, "Updated" },
            { ResponseType.Deleted, "Deleted" },
            { ResponseType.NoContent, "NoContent" },
            { ResponseType.Accepted, "Accepted" },
            { ResponseType.Validation, "Validation" },
            { ResponseType.Problem, "Problem" },
            { ResponseType.NotFound, "NotFound" },
            { ResponseType.Conflict, "Conflict" },
            { ResponseType.Unauthorized, "Unauthorized" },
            { ResponseType.Error, "Error" },
            { ResponseType.BadRequest, "BadRequest" },
            { ResponseType.Unprocessable, "Unprocessable" },
            { ResponseType.Forbidden, "Forbidden" },
            { ResponseType.TooManyRequests, "TooManyRequests" },
            { ResponseType.ServiceUnavailable, "ServiceUnavailable" },
            { ResponseType.GatewayTimeout, "GatewayTimeout" },
        };

    [Theory]
    [MemberData(nameof(BuiltInValues))]
    public void BuiltInValue_ShouldHaveCorrectName(ResponseType responseType, string expectedValue)
    {
        // Assert
        responseType.ToString().Should().Be(expectedValue);
    }

    [Fact]
    public void BuiltInValues_ShouldBeDistinct()
    {
        // Arrange
        ResponseType[] allValues =
        [
            ResponseType.Success, ResponseType.Created, ResponseType.Updated,
            ResponseType.Deleted, ResponseType.NoContent, ResponseType.Accepted,
            ResponseType.Validation, ResponseType.Problem, ResponseType.NotFound,
            ResponseType.Conflict, ResponseType.Unauthorized, ResponseType.Error,
            ResponseType.BadRequest, ResponseType.Unprocessable, ResponseType.Forbidden,
            ResponseType.TooManyRequests, ResponseType.ServiceUnavailable, ResponseType.GatewayTimeout,
        ];

        // Act
        int distinct = allValues.Distinct().Count();

        // Assert
        distinct.Should().Be(allValues.Length);
    }

    // =================================================================
    // ToString
    // =================================================================

    [Theory]
    [MemberData(nameof(BuiltInValues))]
    public void ToString_ShouldReturnMemberName(ResponseType responseType, string expectedValue)
    {
        // Act
        string result = responseType.ToString();

        // Assert
        result.Should().Be(expectedValue);
    }

    // =================================================================
    // JSON Serialization
    // =================================================================

    [Fact]
    public void Serialize_ShouldWritePlainString()
    {
        // Act
        string json = JsonSerializer.Serialize(ResponseType.NotFound);

        // Assert
        json.Should().Be("\"NotFound\"");
    }

    [Theory]
    [MemberData(nameof(BuiltInValues))]
    public void Serialize_AllBuiltInValues_ShouldWritePlainString(ResponseType responseType, string expectedValue)
    {
        // Act
        string json = JsonSerializer.Serialize(responseType);

        // Assert
        json.Should().Be($"\"{expectedValue}\"");
    }

    [Fact]
    public void Serialize_WithIndented_ShouldWritePlainString()
    {
        // Arrange
        var obj = new { Type = ResponseType.NotFound };

        // Act
        string json = JsonSerializer.Serialize(obj, JsonOptions);

        // Assert
        json.Should().Contain("\"NotFound\"");
    }

    // =================================================================
    // JSON Deserialization
    // =================================================================

    [Fact]
    public void Deserialize_ValidString_ShouldReturnMatchingMember()
    {
        // Arrange
        const string json = "\"NotFound\"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.NotFound);
    }

    [Fact]
    public void Deserialize_ValidString_CaseInsensitive_ShouldReturnMatchingMember()
    {
        // Arrange
        const string json = "\"notfound\"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.NotFound);
    }

    [Fact]
    public void Deserialize_Null_ShouldReturnError()
    {
        // .NET 9+ passes null tokens to the converter; the null string falls back to Error.
        const string json = "null";

        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void Deserialize_EmptyString_ShouldReturnError()
    {
        // Arrange
        const string json = "\"\"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void Deserialize_WhitespaceString_ShouldReturnError()
    {
        // Arrange
        const string json = "\"   \"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void Deserialize_UnknownString_ShouldReturnError()
    {
        // Arrange
        const string json = "\"PaymentRequired\"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void RoundTrip_AllBuiltInValues_ShouldPreserveEquality()
    {
        ResponseType[] allValues =
        [
            ResponseType.Success, ResponseType.Created, ResponseType.Updated,
            ResponseType.Deleted, ResponseType.NoContent, ResponseType.Accepted,
            ResponseType.Validation, ResponseType.Problem, ResponseType.NotFound,
            ResponseType.Conflict, ResponseType.Unauthorized, ResponseType.Error,
            ResponseType.BadRequest, ResponseType.Unprocessable, ResponseType.Forbidden,
            ResponseType.TooManyRequests, ResponseType.ServiceUnavailable, ResponseType.GatewayTimeout,
        ];

        foreach (ResponseType responseType in allValues)
        {
            string json = JsonSerializer.Serialize(responseType);
            ResponseType deserialized = JsonSerializer.Deserialize<ResponseType>(json);

            deserialized.Should().Be(responseType, $"round-trip should preserve equality for '{responseType}'");
        }
    }
}
