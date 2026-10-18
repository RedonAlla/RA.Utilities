using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Extensions;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Tests.RA.Utilities.Application.Validation;

/// <summary>
/// Contains unit tests for the <see cref="IbanValidatorExtensions"/> class.
/// </summary>
public class IbanValidatorExtensionsTests
{
    // =================================================================
    // Test models and validators
    // =================================================================

    private sealed class PaymentModel
    {
        public string Iban { get; set; } = string.Empty;
    }

    private sealed class PaymentValidator : AbstractValidator<PaymentModel>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Iban).MustBeValidIban();
        }
    }

    private sealed class BeneficiaryModel
    {
        public string Iban { get; set; } = string.Empty;
    }

    private sealed class BeneficiaryValidator : AbstractValidator<BeneficiaryModel>
    {
        public BeneficiaryValidator()
        {
            RuleFor(x => x.Iban).MustBeValidIban();
        }
    }

    private sealed class RemittanceModel
    {
        public BeneficiaryModel Beneficiary { get; set; } = new();
    }

    private sealed class RemittanceValidator : AbstractValidator<RemittanceModel>
    {
        public RemittanceValidator()
        {
            RuleFor(x => x.Beneficiary).SetValidator(new BeneficiaryValidator());
        }
    }

    // =================================================================
    // MustBeValidIban — valid IBANs
    // =================================================================

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> passes validation for
    /// IBANs the value object accepts, including inputs that only differ by casing or by the
    /// grouping separators of the paper format.
    /// </summary>
    [Theory]
    [InlineData("GB82WEST12345698765432")] // compact
    [InlineData("gb82west12345698765432")] // lowercase
    [InlineData("  gb82 west 1234 5698 7654 32  ")] // paper format with surrounding whitespace
    [InlineData("GB82-WEST-1234-5698-7654-32")] // hyphen separators
    [InlineData("GB82'WEST'1234'5698'7654'32")] // apostrophe separators
    [InlineData("GB82\u00A0WEST\u00A01234\u00A05698\u00A07654\u00A032")] // non-breaking spaces
    [InlineData("DE89 3704 0044 0532 0130 00")] // Germany
    [InlineData("FR14 2004 1010 0505 0001 3M02 606")] // France, alphanumeric BBAN
    [InlineData("NL91 ABNA 0417 1643 00")] // Netherlands
    [InlineData("NO93 8601 1117 947")] // Norway, shortest registry entry
    [InlineData("MT84 MALT 0110 0001 2345 MTLC AST0 01S")] // Malta, longest registry entry
    public void MustBeValidIban_WithValidIban_ShouldPassValidation(string iban)
    {
        // Arrange
        var validator = new PaymentValidator();
        var model = new PaymentModel { Iban = iban };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> reports no failure
    /// through the asynchronous validation path for a valid IBAN.
    /// </summary>
    [Fact]
    public async Task MustBeValidIban_WithValidIban_ShouldPassAsyncValidation()
    {
        // Arrange
        var validator = new PaymentValidator();
        var model = new PaymentModel { Iban = "gb82 west 1234 5698 7654 32" };

        // Act
        ValidationResult result = await validator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // =================================================================
    // MustBeValidIban — invalid IBANs
    // =================================================================

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> fails validation with a
    /// single failure for the property when the value object rejects the IBAN, whether the input is
    /// structurally invalid or has incorrect check digits.
    /// </summary>
    [Theory]
    [InlineData("GB82")] // shorter than the 13-character minimum
    [InlineData("GB8212345678")] // only 8 characters after the header
    [InlineData("GB82WEST123456987654321234567890123")] // more than 30 characters after the header
    [InlineData("G182WEST12345698765432")] // country code must be letters
    [InlineData("GB8AWEST12345698765432")] // check digits must be digits
    [InlineData("GB82_WEST12345698765432")] // underscore is not a valid separator
    [InlineData("GB82WEST1234569876543!")] // punctuation is not alphanumeric
    [InlineData("GB81WEST12345698765432")] // wrong check digits
    [InlineData("DE88370400440532013000")] // wrong check digits for Germany
    [InlineData("US82WEST12345698765432")] // structurally valid, wrong check digits for the country
    public void MustBeValidIban_WithInvalidIban_ShouldFailValidation(string iban)
    {
        // Arrange
        var validator = new PaymentValidator();
        var model = new PaymentModel { Iban = iban };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.PropertyName.Should().Be(nameof(PaymentModel.Iban));
    }

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> reports the generic
    /// <c>BadRequestException</c> message, not the rule-specific value object error, and does
    /// not echo the attempted value back to the caller.
    /// </summary>
    [Theory]
    [InlineData("GB81WEST12345698765432")] // wrong check digits
    [InlineData("GB82")] // structural failure
    public void MustBeValidIban_WithInvalidIban_ShouldReportGenericBadRequestMessage(string iban)
    {
        // Arrange
        var validator = new PaymentValidator();
        var model = new PaymentModel { Iban = iban };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        ValidationFailure failure = result.Errors.Should().ContainSingle().Which;
        failure.ErrorMessage.Should().Be(BaseResponseMessages.BadRequest);
        failure.AttemptedValue.Should().BeNull();
    }

    // =================================================================
    // MustBeValidIban — missing values
    // =================================================================

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> fails validation for
    /// null, empty and whitespace-only values, so the rule doubles as a required check instead of
    /// leaving requiredness to <c>NotNull()</c> or <c>NotEmpty()</c>.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\r\n")]
    public void MustBeValidIban_WithMissingIban_ShouldFailValidation(string? iban)
    {
        // Arrange
        var validator = new PaymentValidator();
        var model = new PaymentModel { Iban = iban! };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.PropertyName.Should().Be(nameof(PaymentModel.Iban));
    }

    // =================================================================
    // MustBeValidIban — failure property path
    // =================================================================

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> attributes the failure to
    /// the full property path of a nested rule, not just the declaring property name.
    /// </summary>
    [Fact]
    public void MustBeValidIban_WithInvalidNestedIban_ShouldReportFullPropertyPath()
    {
        // Arrange
        var validator = new RemittanceValidator();
        var model = new RemittanceModel
        {
            Beneficiary = new BeneficiaryModel { Iban = "GB81WEST12345698765432" }
        };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.PropertyName.Should().Be($"{nameof(RemittanceModel.Beneficiary)}.{nameof(BeneficiaryModel.Iban)}");
    }

    /// <summary>
    /// Tests that <see cref="IbanValidatorExtensions.MustBeValidIban{T}"/> leaves validation of a
    /// valid nested IBAN, and of every other property, untouched.
    /// </summary>
    [Fact]
    public void MustBeValidIban_WithValidNestedIban_ShouldPassValidation()
    {
        // Arrange
        var validator = new RemittanceValidator();
        var model = new RemittanceModel
        {
            Beneficiary = new BeneficiaryModel { Iban = "GB82WEST12345698765432" }
        };

        // Act
        ValidationResult result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
