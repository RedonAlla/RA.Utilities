using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Tests.RA.Utilities.Core.Constants;

/// <summary>
/// Contains unit tests for the <see cref="ResponseType"/> enum, covering its numeric values,
/// member names, and JSON behavior with and without <see cref="JsonStringEnumConverter"/>.
/// </summary>
public class ResponseTypeTests
{
    private static readonly JsonSerializerOptions DefaultOptions = new();

    private static readonly JsonSerializerOptions StringEnumOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Provides all built-in <see cref="ResponseType"/> members with their documented
    /// numeric values and names.
    /// </summary>
    public static TheoryData<ResponseType, int, string> BuiltInValues =>
        new()
        {
            { ResponseType.Success, 0, "Success" },
            { ResponseType.Created, 1, "Created" },
            { ResponseType.Updated, 2, "Updated" },
            { ResponseType.Deleted, 3, "Deleted" },
            { ResponseType.NoContent, 4, "NoContent" },
            { ResponseType.Accepted, 5, "Accepted" },
            { ResponseType.Validation, 6, "Validation" },
            { ResponseType.Problem, 7, "Problem" },
            { ResponseType.NotFound, 8, "NotFound" },
            { ResponseType.Conflict, 9, "Conflict" },
            { ResponseType.Unauthorized, 10, "Unauthorized" },
            { ResponseType.Error, 11, "Error" },
            { ResponseType.BadRequest, 12, "BadRequest" },
            { ResponseType.Unprocessable, 13, "Unprocessable" },
            { ResponseType.Forbidden, 14, "Forbidden" },
            { ResponseType.TooManyRequests, 15, "TooManyRequests" },
            { ResponseType.ServiceUnavailable, 16, "ServiceUnavailable" },
            { ResponseType.GatewayTimeout, 17, "GatewayTimeout" },
        };

    // =================================================================
    // Enum contract
    // =================================================================

    [Theory]
    [MemberData(nameof(BuiltInValues))]
    public void BuiltInValue_ShouldMatchDocumentedContract(
        ResponseType responseType,
        int expectedValue,
        string expectedName)
    {
        // Numeric value and member name
        ((int)responseType).Should().Be(expectedValue);
        responseType.ToString().Should().Be(expectedName);

        // Default System.Text.Json options write the numeric value
        string defaultJson = JsonSerializer.Serialize(responseType, DefaultOptions);
        defaultJson.Should().Be(expectedValue.ToString());
        JsonSerializer.Deserialize<ResponseType>(defaultJson, DefaultOptions).Should().Be(responseType);

        // JsonStringEnumConverter writes and reads the member name
        string namedJson = JsonSerializer.Serialize(responseType, StringEnumOptions);
        namedJson.Should().Be($"\"{expectedName}\"");
        JsonSerializer.Deserialize<ResponseType>(namedJson, StringEnumOptions).Should().Be(responseType);
    }

    [Fact]
    public void DefinedValues_ShouldBeUnique()
    {
        // Act
        ResponseType[] allValues = Enum.GetValues<ResponseType>();

        // Assert
        allValues.Should().OnlyHaveUniqueItems();
    }

    // =================================================================
    // JSON deserialization - JsonStringEnumConverter
    // =================================================================

    [Fact]
    public void Deserialize_WithStringEnumConverter_ShouldMatchNameCaseInsensitively()
    {
        // Arrange
        const string json = "\"notfound\"";

        // Act
        ResponseType result = JsonSerializer.Deserialize<ResponseType>(json, StringEnumOptions);

        // Assert
        result.Should().Be(ResponseType.NotFound);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("\"\"")]
    [InlineData("\"   \"")]
    [InlineData("\"PaymentRequired\"")]
    public void Deserialize_WithStringEnumConverter_InvalidInput_ShouldThrow(string json)
    {
        // Act
        Action act = () => JsonSerializer.Deserialize<ResponseType>(json, StringEnumOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }
}
