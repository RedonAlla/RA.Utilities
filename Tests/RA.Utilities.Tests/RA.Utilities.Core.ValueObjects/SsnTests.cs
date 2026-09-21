using System;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Tests.RA.Utilities.Core.ValueObjects;

/// <summary>
/// Contains unit tests for the <see cref="SSN"/> value object.
/// </summary>
public class SsnTests
{
    // =================================================================
    // Constructor and normalization
    // =================================================================

    /// <summary>
    /// Provides valid SSNs and their expected normalized form.
    /// </summary>
    public static TheoryData<string, string> ValidSsns => new()
    {
        { "J01234567R", "J01234567R" },
        { "  j01234567r  ", "J01234567R" },
        { "X98765432Z", "X98765432Z" },
    };

    [Theory]
    [MemberData(nameof(ValidSsns))]
    public void Constructor_ValidInput_ShouldNormalizeValue(string input, string expected)
    {
        // Act
        var ssn = new SSN(input);

        // Assert
        ssn.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_MissingInput_ShouldThrowRequiredError(string? input)
    {
        // Act
        Func<SSN> act = () => new SSN(input!);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.ErrorCode.Should().Be(BaseResponseCode.BadRequest);
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.SsnRequired);
        exception.Errors[0].PropertyName.Should().Be("SSN");
    }

    [Theory]
    [InlineData("J0123456789R")]
    [InlineData("J012345678901R")]
    public void Constructor_InputExceedingMaxLength_ShouldThrowLengthError(string input)
    {
        // Act
        Func<SSN> act = () => new SSN(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.SsnLength);
        exception.Errors[0].AttemptedValue.Should().Be(input.Trim().ToUpperInvariant());
    }

    [Theory]
    [InlineData("0123456789")] // letters missing
    [InlineData("J0123456R")]  // too short for the format, within max length
    [InlineData("JJ1234567R")] // non-digit in the second position
    public void Constructor_InvalidFormat_ShouldThrowNotValidError(string input)
    {
        // Act
        Func<SSN> act = () => new SSN(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.SsnNotValid);
        exception.Errors[0].ErrorMessage.Should().Be(BaseErrorMessage.SsnNotValid);
    }

    // =================================================================
    // Equality
    // =================================================================

    [Fact]
    public void Ssns_WithSameNormalizedValue_ShouldBeEqual()
    {
        // Act & Assert — casing/whitespace are normalized away
        new SSN(" j01234567r ").Should().Be(new SSN("J01234567R"));
        (new SSN("j01234567r") == new SSN("J01234567R")).Should().BeTrue();
    }

    [Fact]
    public void Ssns_WithDifferentValues_ShouldNotBeEqual()
    {
        // Act & Assert
        (new SSN("J01234567R") == new SSN("X98765432Z")).Should().BeFalse();
    }

    // =================================================================
    // Parse and TryParse
    // =================================================================

    [Fact]
    public void Parse_ValidInput_ShouldReturnNormalizedSsn()
    {
        // Act
        var ssn = SSN.Parse("j01234567r", provider: null);

        // Assert
        ssn.Value.Should().Be("J01234567R");
    }

    [Fact]
    public void Parse_InvalidInput_ShouldThrowBadRequestException()
    {
        // Act
        Func<SSN> act = () => SSN.Parse("0123456789", provider: null);

        // Assert
        act.Should().Throw<BadRequestException>();
    }

    [Fact]
    public void TryParse_ValidInput_ShouldReturnTrueAndSsn()
    {
        // Act
        bool success = SSN.TryParse("j01234567r", provider: null, out SSN result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new SSN("J01234567R"));
    }

    [Fact]
    public void TryParse_InvalidInput_ShouldReturnFalseAndNull()
    {
        // Act
        bool success = SSN.TryParse("0123456789", provider: null, out SSN result);

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
        string value = new SSN("J01234567R");

        // Assert
        value.Should().Be("J01234567R");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Act & Assert
        new SSN("J01234567R").ToString().Should().Be("J01234567R");
    }

    // =================================================================
    // JSON serialization (via ValueObjectJsonConverter<SSN>)
    // =================================================================

    [Fact]
    public void Serialize_ShouldWritePlainString()
    {
        // Act
        string json = JsonSerializer.Serialize(new SSN("J01234567R"));

        // Assert
        json.Should().Be("\"J01234567R\"");
    }

    [Fact]
    public void Deserialize_ValidString_ShouldCreateNormalizedSsn()
    {
        // Act
        SSN? ssn = JsonSerializer.Deserialize<SSN>("\"j01234567r\"");

        // Assert
        ssn.Should().Be(new SSN("J01234567R"));
    }

    [Fact]
    public void Deserialize_InvalidString_ShouldThrowJsonException()
    {
        // Act
        Func<SSN?> act = () => JsonSerializer.Deserialize<SSN>("\"0123456789\"");

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void RoundTrip_ShouldPreserveEquality()
    {
        // Arrange
        var original = new SSN("J01234567R");

        // Act
        SSN? deserialized = JsonSerializer.Deserialize<SSN>(JsonSerializer.Serialize(original));

        // Assert
        deserialized.Should().Be(original);
    }
}
