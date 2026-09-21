using System;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.ValueObjects;
using RA.Utilities.Core.ValueObjects.Converters;

namespace RA.Utilities.Tests.RA.Utilities.Core.ValueObjects;

/// <summary>
/// Contains unit tests for the generic <see cref="ValueObjectJsonConverter{T}"/>
/// shared by all value objects.
/// </summary>
public class ValueObjectJsonConverterTests
{
    /// <summary>
    /// Provides every value object type the generic converter supports.
    /// </summary>
    public static TheoryData<Type> ValueObjectTypes => new()
    {
        typeof(Email),
        typeof(Currency),
        typeof(SSN),
    };

    [Fact]
    public void Deserialize_NonStringToken_ShouldThrowJsonExceptionWithTypeName()
    {
        // Act
        Func<Currency?> act = () => JsonSerializer.Deserialize<Currency>("123");

        // Assert
        act.Should().Throw<JsonException>()
            .WithMessage("Currency must be a JSON string.");
    }

    [Theory]
    [MemberData(nameof(ValueObjectTypes))]
    public void Deserialize_WhitespaceString_ShouldReturnNull(Type type)
    {
        // Act
        object? result = JsonSerializer.Deserialize("\"   \"", type);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(ValueObjectTypes))]
    public void Deserialize_NullToken_ShouldReturnNull(Type type)
    {
        // Act
        object? result = JsonSerializer.Deserialize("null", type);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Serialize_AsObjectProperty_ShouldWritePlainString()
    {
        // Arrange
        var payload = new { Email = new Email("User@Gmail.com"), Currency = new Currency("eur") };

        // Act
        string json = JsonSerializer.Serialize(payload);

        // Assert
        json.Should().Contain("\"Email\":\"user@gmail.com\"");
        json.Should().Contain("\"Currency\":\"EUR\"");
    }
}
