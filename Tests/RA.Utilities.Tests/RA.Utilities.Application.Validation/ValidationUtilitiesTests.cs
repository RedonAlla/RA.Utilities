using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Utilities;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Tests.RA.Utilities.Application.Validation;

/// <summary>
/// Contains unit tests for the <see cref="ValidationUtilities"/> class.
/// </summary>
public class ValidationUtilitiesTests
{
    // =================================================================
    // Test model and validators
    // =================================================================

    private sealed class TestRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private sealed class NameRequiredValidator : AbstractValidator<TestRequest>
    {
        public NameRequiredValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }

    private sealed class AgePositiveValidator : AbstractValidator<TestRequest>
    {
        public AgePositiveValidator()
        {
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0.");
        }
    }

    private sealed class TwoRuleValidator : AbstractValidator<TestRequest>
    {
        public TwoRuleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0.");
        }
    }

    private sealed class TrackingAsyncValidator : AbstractValidator<TestRequest>
    {
        public bool FirstRuleExecuted { get; private set; }
        public bool SecondRuleExecuted { get; private set; }

        public TrackingAsyncValidator()
        {
            RuleFor(x => x.Name).MustAsync(async (_, cancellationToken) =>
            {
                await Task.Delay(10, cancellationToken);
                FirstRuleExecuted = true;
                return false;
            });

            RuleFor(x => x.Age).MustAsync(async (_, cancellationToken) =>
            {
                await Task.Delay(10, cancellationToken);
                SecondRuleExecuted = true;
                return false;
            });
        }
    }

    // =================================================================
    // ValidateAsync tests
    // =================================================================

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> returns an empty
    /// array when no validators are supplied.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithNoValidators_ShouldReturnEmptyArray()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(request, []);

        // Assert
        failures.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> returns an empty
    /// array when the request is valid.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithValidRequest_ShouldReturnEmptyArray()
    {
        // Arrange
        var request = new TestRequest { Name = "John", Age = 25 };

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(
            request, new IValidator<TestRequest>[] { new NameRequiredValidator(), new AgePositiveValidator() });

        // Assert
        failures.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> returns the
    /// validation failures when the request is invalid.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithInvalidRequest_ShouldReturnFailures()
    {
        // Arrange
        var request = new TestRequest { Name = string.Empty, Age = 25 };

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(request, [new NameRequiredValidator()]);

        // Assert
        failures.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("Name is required.");
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> aggregates failures
    /// from all validators and preserves the order of the validators.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithMultipleFailingValidators_ShouldAggregateFailuresInValidatorOrder()
    {
        // Arrange
        var request = new TestRequest { Name = string.Empty, Age = 0 };

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(
            request, new IValidator<TestRequest>[] { new NameRequiredValidator(), new AgePositiveValidator() });

        // Assert
        failures.Should().HaveCount(2);
        failures[0].PropertyName.Should().Be("Name");
        failures[1].PropertyName.Should().Be("Age");
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> returns only the
    /// failures from validators that did not pass.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithMixedValidators_ShouldReturnOnlyFailures()
    {
        // Arrange
        var request = new TestRequest { Name = "John", Age = 0 };

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(
            request, new IValidator<TestRequest>[] { new NameRequiredValidator(), new AgePositiveValidator() });

        // Assert
        failures.Should().ContainSingle()
            .Which.PropertyName.Should().Be("Age");
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> returns all failures
    /// when a single validator has multiple failing rules.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithMultipleFailingRulesInOneValidator_ShouldReturnAllFailures()
    {
        // Arrange
        var request = new TestRequest { Name = string.Empty, Age = 0 };

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(request, [new TwoRuleValidator()]);

        // Assert
        failures.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.ValidateAsync{TRequest}"/> awaits all async
    /// validators before returning.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_WithAsyncValidators_ShouldAwaitAllValidators()
    {
        // Arrange
        var validator = new TrackingAsyncValidator();
        var request = new TestRequest();

        // Act
        ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(request, [validator]);

        // Assert
        failures.Should().HaveCount(2);
        validator.FirstRuleExecuted.Should().BeTrue();
        validator.SecondRuleExecuted.Should().BeTrue();
    }

    // =================================================================
    // CreateValidationErrorResult tests
    // =================================================================

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.CreateValidationErrorResult"/> maps all
    /// failure properties onto the resulting <see cref="ValidationError"/>.
    /// </summary>
    [Fact]
    public void CreateValidationErrorResult_WithFailure_ShouldMapAllProperties()
    {
        // Arrange
        var failure = new ValidationFailure("Name", "Name is required.")
        {
            AttemptedValue = "abc",
            ErrorCode = "CustomCode",
        };

        // Act
        BadRequestException exception = ValidationUtilities.CreateValidationErrorResult([failure]);

        // Assert
        exception.Errors.Should().ContainSingle();
        ValidationError error = exception.Errors[0];
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("Name is required.");
        error.AttemptedValue.Should().Be("abc");
        error.ErrorCode.Should().Be("CustomCode");
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.CreateValidationErrorResult"/> preserves the
    /// order of the supplied failures.
    /// </summary>
    [Fact]
    public void CreateValidationErrorResult_WithMultipleFailures_ShouldPreserveOrder()
    {
        // Arrange
        ValidationFailure[] failures =
        [
            new ValidationFailure("Name", "Name is required."),
            new ValidationFailure("Age", "Age must be greater than 0."),
        ];

        // Act
        BadRequestException exception = ValidationUtilities.CreateValidationErrorResult(failures);

        // Assert
        exception.Errors.Should().HaveCount(2);
        exception.Errors[0].PropertyName.Should().Be("Name");
        exception.Errors[1].PropertyName.Should().Be("Age");
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.CreateValidationErrorResult"/> returns an
    /// exception with an empty error collection when no failures are supplied.
    /// </summary>
    [Fact]
    public void CreateValidationErrorResult_WithNoFailures_ShouldReturnExceptionWithEmptyErrors()
    {
        // Act
        BadRequestException exception = ValidationUtilities.CreateValidationErrorResult([]);

        // Assert
        exception.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that <see cref="ValidationUtilities.CreateValidationErrorResult"/> leaves optional
    /// failure data null when it was not supplied.
    /// </summary>
    [Fact]
    public void CreateValidationErrorResult_WithFailureWithoutOptionalData_ShouldMapNulls()
    {
        // Arrange
        var failure = new ValidationFailure("Name", "Name is required.");

        // Act
        BadRequestException exception = ValidationUtilities.CreateValidationErrorResult([failure]);

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].AttemptedValue.Should().BeNull();
    }
}
