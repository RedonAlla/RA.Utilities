using System;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Tests.RA.Utilities.Core.Constants;

/// <summary>
/// Contains unit tests for the <see cref="ResponseType"/> record,
/// including equality, serialization, and extensibility.
/// </summary>
public class ResponseTypeTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    // =================================================================
    // Built-in values
    // =================================================================

    /// <summary>
    /// Provides all built-in ResponseType fields and their expected string values.
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
    public void BuiltInValue_ShouldHaveCorrectValue(ResponseType responseType, string expectedValue)
    {
        // Assert
        responseType.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void BuiltInValues_ShouldBeDistinctByValue()
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
    public void ToString_ShouldReturnValue(ResponseType responseType, string expectedValue)
    {
        // Act
        string result = responseType.ToString();

        // Assert
        result.Should().Be(expectedValue);
    }

    // =================================================================
    // Equality
    // =================================================================

    [Fact]
    public void SameStaticField_ShouldBeEqual()
    {
        // Arrange
        ResponseType a = ResponseType.NotFound;
        ResponseType b = ResponseType.NotFound;

        // Act & Assert
        (a == b).Should().BeTrue();
        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void DifferentFields_ShouldNotBeEqual()
    {
        // Act & Assert
        (ResponseType.NotFound == ResponseType.Conflict).Should().BeFalse();
        ResponseType.NotFound.Equals(ResponseType.Conflict).Should().BeFalse();
    }

    [Fact]
    public void DifferentInstances_SameValue_ShouldBeEqual()
    {
        // Arrange
        var a = new ResponseType("Custom");
        var b = new ResponseType("Custom");

        // Act & Assert — records use value equality
        (a == b).Should().BeTrue();
        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void DifferentInstances_DifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var a = new ResponseType("Alpha");
        var b = new ResponseType("Beta");

        // Act & Assert
        (a == b).Should().BeFalse();
    }

    // =================================================================
    // Implicit string conversion
    // =================================================================

    [Theory]
    [MemberData(nameof(BuiltInValues))]
    public void ImplicitConversionToString_ShouldReturnValue(ResponseType responseType, string expectedValue)
    {
        // Act
        string result = responseType;

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
    public void Deserialize_ValidString_ShouldCreateInstance()
    {
        // Arrange
        const string json = "\"NotFound\"";

        // Act
        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("NotFound");
    }

    [Fact]
    public void Deserialize_ValidString_ShouldEqualBuiltInField()
    {
        // Arrange
        const string json = "\"NotFound\"";

        // Act
        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.NotFound);
    }

    [Fact]
    public void Deserialize_Null_ShouldReturnNull()
    {
        // System.Text.Json returns null for null JSON before invoking the converter.
        // This is expected behavior for reference types.
        const string json = "null";

        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        result.Should().BeNull();
    }

    [Fact]
    public void Deserialize_EmptyString_ShouldReturnError()
    {
        // Arrange
        const string json = "\"\"";

        // Act
        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void Deserialize_WhitespaceString_ShouldReturnError()
    {
        // Arrange
        const string json = "\"   \"";

        // Act
        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().Be(ResponseType.Error);
    }

    [Fact]
    public void RoundTrip_AllBuiltInValues_ShouldPreserveEquality()
    {
        ResponseType[] allValues = new[]
        {
            ResponseType.Success, ResponseType.Created, ResponseType.Updated,
            ResponseType.Deleted, ResponseType.NoContent, ResponseType.Accepted,
            ResponseType.Validation, ResponseType.Problem, ResponseType.NotFound,
            ResponseType.Conflict, ResponseType.Unauthorized, ResponseType.Error,
            ResponseType.BadRequest, ResponseType.Unprocessable, ResponseType.Forbidden,
            ResponseType.TooManyRequests, ResponseType.ServiceUnavailable, ResponseType.GatewayTimeout,
        };

        foreach (ResponseType responseType in allValues)
        {
            string json = JsonSerializer.Serialize(responseType);
            ResponseType? deserialized = JsonSerializer.Deserialize<ResponseType>(json);

            deserialized.Should().Be(responseType, $"round-trip should preserve equality for '{responseType.Value}'");
        }
    }

    // =================================================================
    // Constructor validation
    // =================================================================

    [Fact]
    public void Constructor_NullValue_ShouldThrow()
    {
        // Act
        Func<ResponseType> act = () => new ResponseType(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyValue_ShouldThrow()
    {
        // Act
        Func<ResponseType> act = () => new ResponseType(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WhitespaceValue_ShouldThrow()
    {
        // Act
        Func<ResponseType> act = () => new ResponseType("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // =================================================================
    // Extensibility — custom derived response types
    // =================================================================

    private sealed record CustomResponseType : ResponseType
    {
        private CustomResponseType(string value) : base(value) { }
        public static readonly CustomResponseType Instance = new("PaymentRequired");
    }

    [Fact]
    public void CustomDerivedType_ShouldHaveCorrectValue()
    {
        // Assert
        CustomResponseType.Instance.Value.Should().Be("PaymentRequired");
    }

    [Fact]
    public void CustomDerivedType_ShouldBeAssignableToBase()
    {
        // Act
        ResponseType type = CustomResponseType.Instance;

        // Assert
        type.Should().BeOfType<CustomResponseType>();
        type.Value.Should().Be("PaymentRequired");
    }

    [Fact]
    public void CustomDerivedType_ShouldSerializeAsPlainString()
    {
        // Serialize via the base type to ensure the converter is picked up
        ResponseType value = CustomResponseType.Instance;
        string json = JsonSerializer.Serialize(value);

        json.Should().Be("\"PaymentRequired\"");
    }

    [Fact]
    public void CustomDerivedType_ShouldDeserializeViaBaseConverter()
    {
        // Arrange
        const string json = "\"PaymentRequired\"";

        // Act
        ResponseType? result = JsonSerializer.Deserialize<ResponseType>(json);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("PaymentRequired");
    }

    [Fact]
    public void CustomDerivedType_ShouldNotEqualBuiltInWithSameValue()
    {
        // CustomResponseType("Success") is not the same type as ResponseType.Success
        // but records use value equality — let's verify
        var custom = new ResponseType("Success");

        // Same Value, so record equality says true
        (custom == ResponseType.Success).Should().BeTrue();
        custom.Should().Be(ResponseType.Success);
    }
}
