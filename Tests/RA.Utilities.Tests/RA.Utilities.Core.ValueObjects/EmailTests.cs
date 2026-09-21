using System;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Tests.RA.Utilities.Core.ValueObjects;

/// <summary>
/// Contains unit tests for the <see cref="Email"/> value object.
/// </summary>
public class EmailTests
{
    // =================================================================
    // Constructor and normalization
    // =================================================================

    /// <summary>
    /// Provides valid email addresses and their expected normalized form.
    /// </summary>
    public static TheoryData<string, string> ValidEmails => new()
    {
        { "user@gmail.com", "user@gmail.com" },
        { "  User@Gmail.com  ", "user@gmail.com" },
        { "USER@EXAMPLE.COM", "user@example.com" },
        { "user.name+tag@sub.domain.com", "user.name+tag@sub.domain.com" },
    };

    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void Constructor_ValidInput_ShouldNormalizeValue(string input, string expected)
    {
        // Act
        var email = new Email(input);

        // Assert
        email.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_MissingInput_ShouldThrowRequiredError(string? input)
    {
        // Act
        Func<Email> act = () => new Email(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.ErrorCode.Should().Be(BaseResponseCode.BadRequest);
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.EmailRequired);
        exception.Errors[0].PropertyName.Should().Be("Email");
    }

    [Fact]
    public void Constructor_InputExceedingMaxLength_ShouldThrowMaxLengthError()
    {
        // Arrange — 260 characters, RFC 5321 limit is 254
        string input = $"{new string('a', 250)}@gmail.com";

        // Act
        Func<Email> act = () => new Email(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.EmailMaxLength);
        exception.Errors[0].AttemptedValue.Should().Be(input);
    }

    [Theory]
    [InlineData("usergmail.com")]
    [InlineData("user@gmail")]
    [InlineData("user @gmail.com")]
    [InlineData("user@@gmail.com")]
    [InlineData("not-an-email")]
    public void Constructor_InvalidFormat_ShouldThrowNotValidError(string input)
    {
        // Act
        Func<Email> act = () => new Email(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.EmailNotValid);
        exception.Errors[0].ErrorMessage.Should().Be(BaseErrorMessage.EmailNotValid);
    }

    // =================================================================
    // LocalPart and Domain
    // =================================================================

    [Fact]
    public void LocalPart_ShouldReturnPartBeforeAtSign()
    {
        // Act
        var email = new Email("user@gmail.com");

        // Assert
        email.LocalPart.Should().Be("user");
    }

    [Fact]
    public void Domain_ShouldReturnPartAfterAtSign()
    {
        // Act
        var email = new Email("user@gmail.com");

        // Assert
        email.Domain.Should().Be("gmail.com");
    }

    // =================================================================
    // Equality
    // =================================================================

    [Fact]
    public void Emails_WithSameNormalizedValue_ShouldBeEqual()
    {
        // Act & Assert — records use value equality, casing/whitespace are normalized away
        new Email(" User@Gmail.com ").Should().Be(new Email("user@gmail.com"));
        (new Email("a@b.com") == new Email("A@B.COM")).Should().BeTrue();
    }

    [Fact]
    public void Emails_WithDifferentValues_ShouldNotBeEqual()
    {
        // Act & Assert
        (new Email("a@b.com") == new Email("a@c.com")).Should().BeFalse();
    }

    // =================================================================
    // Parse and TryParse
    // =================================================================

    [Fact]
    public void Parse_ValidInput_ShouldReturnNormalizedEmail()
    {
        // Act
        var email = Email.Parse("User@Gmail.com", provider: null);

        // Assert
        email.Value.Should().Be("user@gmail.com");
    }

    [Fact]
    public void Parse_InvalidInput_ShouldThrowBadRequestException()
    {
        // Act
        Func<Email> act = () => Email.Parse("not-an-email", provider: null);

        // Assert
        act.Should().Throw<BadRequestException>();
    }

    [Fact]
    public void TryParse_ValidInput_ShouldReturnTrueAndEmail()
    {
        // Act
        bool success = Email.TryParse("user@gmail.com", provider: null, out Email result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new Email("user@gmail.com"));
    }

    [Fact]
    public void TryParse_InvalidInput_ShouldReturnFalseAndNull()
    {
        // Act
        bool success = Email.TryParse("not-an-email", provider: null, out Email result);

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
        string value = new Email("user@gmail.com");

        // Assert
        value.Should().Be("user@gmail.com");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Act & Assert
        new Email("user@gmail.com").ToString().Should().Be("user@gmail.com");
    }

    // =================================================================
    // JSON serialization (via ValueObjectJsonConverter<Email>)
    // =================================================================

    [Fact]
    public void Serialize_ShouldWritePlainString()
    {
        // Act
        string json = JsonSerializer.Serialize(new Email("user@gmail.com"));

        // Assert
        json.Should().Be("\"user@gmail.com\"");
    }

    [Fact]
    public void Deserialize_ValidString_ShouldCreateNormalizedEmail()
    {
        // Act
        Email? email = JsonSerializer.Deserialize<Email>("\" User@Gmail.com \"");

        // Assert
        email.Should().Be(new Email("user@gmail.com"));
    }

    [Fact]
    public void Deserialize_InvalidString_ShouldThrowJsonException()
    {
        // Act
        Func<Email?> act = () => JsonSerializer.Deserialize<Email>("\"not-an-email\"");

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void RoundTrip_ShouldPreserveEquality()
    {
        // Arrange
        var original = new Email("user@gmail.com");

        // Act
        Email? deserialized = JsonSerializer.Deserialize<Email>(JsonSerializer.Serialize(original));

        // Assert
        deserialized.Should().Be(original);
    }
}
