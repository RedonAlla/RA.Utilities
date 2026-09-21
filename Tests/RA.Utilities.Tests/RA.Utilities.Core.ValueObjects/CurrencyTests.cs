using System;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Tests.RA.Utilities.Core.ValueObjects;

/// <summary>
/// Contains unit tests for the <see cref="Currency"/> value object.
/// </summary>
public class CurrencyTests
{
    // =================================================================
    // Constructor and normalization
    // =================================================================

    /// <summary>
    /// Provides valid currency codes and their expected normalized form.
    /// </summary>
    public static TheoryData<string, string> ValidCurrencies => new()
    {
        { "EUR", "EUR" },
        { "usd", "USD" },
        { "  eur  ", "EUR" },
    };

    [Theory]
    [MemberData(nameof(ValidCurrencies))]
    public void Constructor_ValidInput_ShouldNormalizeValue(string input, string expected)
    {
        // Act
        var currency = new Currency(input);

        // Assert
        currency.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_MissingInput_ShouldThrowRequiredError(string? input)
    {
        // Act
        Func<Currency> act = () => new Currency(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.ErrorCode.Should().Be(BaseResponseCode.BadRequest);
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.CurrencyRequired);
        exception.Errors[0].PropertyName.Should().Be("Currency");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("EU")]
    [InlineData("USDD")]
    [InlineData("EURO")]
    public void Constructor_WrongLength_ShouldThrowLengthError(string input)
    {
        // Act
        Func<Currency> act = () => new Currency(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.CurrencyLength);
        exception.Errors[0].AttemptedValue.Should().Be(input.Trim().ToUpperInvariant());
    }

    // =================================================================
    // Equality
    // =================================================================

    [Fact]
    public void Currencies_WithSameNormalizedValue_ShouldBeEqual()
    {
        // Act & Assert — casing/whitespace are normalized away
        new Currency(" eur ").Should().Be(new Currency("EUR"));
        (new Currency("usd") == new Currency("USD")).Should().BeTrue();
    }

    [Fact]
    public void Currencies_WithDifferentValues_ShouldNotBeEqual()
    {
        // Act & Assert
        (new Currency("EUR") == new Currency("USD")).Should().BeFalse();
    }

    // =================================================================
    // Parse and TryParse
    // =================================================================

    [Fact]
    public void Parse_ValidInput_ShouldReturnNormalizedCurrency()
    {
        // Act
        var currency = Currency.Parse("usd", provider: null);

        // Assert
        currency.Value.Should().Be("USD");
    }

    [Fact]
    public void Parse_InvalidInput_ShouldThrowBadRequestException()
    {
        // Act
        Func<Currency> act = () => Currency.Parse("EU", provider: null);

        // Assert
        act.Should().Throw<BadRequestException>();
    }

    [Fact]
    public void TryParse_ValidInput_ShouldReturnTrueAndCurrency()
    {
        // Act
        bool success = Currency.TryParse("eur", provider: null, out Currency result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new Currency("EUR"));
    }

    [Fact]
    public void TryParse_InvalidInput_ShouldReturnFalseAndNull()
    {
        // Act
        bool success = Currency.TryParse("EURO", provider: null, out Currency result);

        // Assert
        success.Should().BeFalse();
        result.Should().BeNull();
    }

    // =================================================================
    // Conversions
    // =================================================================

    [Fact]
    public void ImplicitConversionToString_ShouldReturnValue()
    {
        // Act
        string value = new Currency("EUR");

        // Assert
        value.Should().Be("EUR");
    }

    // =================================================================
    // JSON serialization (via ValueObjectJsonConverter<Currency>)
    // =================================================================

    [Fact]
    public void Serialize_ShouldWritePlainString()
    {
        // Act
        string json = JsonSerializer.Serialize(new Currency("EUR"));

        // Assert
        json.Should().Be("\"EUR\"");
    }

    [Fact]
    public void Deserialize_ValidString_ShouldCreateNormalizedCurrency()
    {
        // Act
        Currency? currency = JsonSerializer.Deserialize<Currency>("\"usd\"");

        // Assert
        currency.Should().Be(new Currency("USD"));
    }

    [Fact]
    public void Deserialize_InvalidString_ShouldThrowJsonException()
    {
        // Act
        Func<Currency?> act = () => JsonSerializer.Deserialize<Currency>("\"EURO\"");

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void RoundTrip_ShouldPreserveEquality()
    {
        // Arrange
        var original = new Currency("EUR");

        // Act
        Currency? deserialized = JsonSerializer.Deserialize<Currency>(JsonSerializer.Serialize(original));

        // Assert
        deserialized.Should().Be(original);
    }
}
