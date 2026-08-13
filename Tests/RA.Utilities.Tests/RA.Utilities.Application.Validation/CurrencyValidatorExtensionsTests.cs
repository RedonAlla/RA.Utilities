using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Application.Validation;

/// <summary>
/// Contains unit tests for the <see cref="CurrencyValidatorExtensions"/> class.
/// </summary>
public class CurrencyValidatorExtensionsTests
{
    // =================================================================
    // Test model and validator
    // =================================================================

    private sealed class CurrencyModel
    {
        public string Currency { get; set; } = string.Empty;
    }

    private sealed class CurrencyValidator : AbstractValidator<CurrencyModel>
    {
        public CurrencyValidator()
        {
            RuleFor(x => x.Currency).MustMatchesCurrencyFormat();
        }
    }

    // =================================================================
    // MustMatchesCurrencyFormat — valid currency codes
    // =================================================================

    /// <summary>
    /// Tests that <see cref="CurrencyValidatorExtensions.MustMatchesCurrencyFormat{T}"/> passes
    /// validation for well-formed ISO 4217 currency codes.
    /// </summary>
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("XAU")]
    public void MustMatchesCurrencyFormat_WithValidCurrency_ShouldPassValidation(string currency)
    {
        // Arrange
        var validator = new CurrencyValidator();
        var model = new CurrencyModel { Currency = currency };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // =================================================================
    // MustMatchesCurrencyFormat — invalid currency codes
    // =================================================================

    /// <summary>
    /// Tests that <see cref="CurrencyValidatorExtensions.MustMatchesCurrencyFormat{T}"/> fails
    /// validation with the expected message for malformed currency codes.
    /// </summary>
    [Theory]
    [InlineData("usd")] // lowercase
    [InlineData("Usd")] // mixed case
    [InlineData("US")] // too short
    [InlineData("USDD")] // too long
    [InlineData("U5D")] // contains a digit
    [InlineData("U$D")] // contains a symbol
    [InlineData("US D")] // contains whitespace
    [InlineData("")] // empty
    public void MustMatchesCurrencyFormat_WithInvalidCurrency_ShouldFailValidation(string currency)
    {
        // Arrange
        var validator = new CurrencyValidator();
        var model = new CurrencyModel { Currency = currency };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("Currency must be 3 uppercase letters (A-Z).");
    }

    /// <summary>
    /// Tests that <see cref="CurrencyValidatorExtensions.MustMatchesCurrencyFormat{T}"/> skips
    /// validation for null values, leaving null handling to explicit requiredness rules.
    /// </summary>
    [Fact]
    public void MustMatchesCurrencyFormat_WithNullCurrency_ShouldPassValidation()
    {
        // Arrange
        var validator = new CurrencyValidator();
        var model = new CurrencyModel { Currency = null! };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
