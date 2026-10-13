using System;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Application.Validation.Extensions;

/// <summary>
/// Provides extension methods for validating currency codes using FluentValidation.
/// </summary>
public static partial class CurrencyValidatorExtensions
{
    private static bool IsValid(string value) =>
        value is null || CurrencyRegex().IsMatch(value);

    /// <summary>
    /// Compiled regex for currency validation (ISO 4217: 3 uppercase letters).
    /// </summary>
    [GeneratedRegex("^[A-Z]{3}$", RegexOptions.Compiled)]
    private static partial Regex CurrencyRegex();

    extension<T>(IRuleBuilder<T, string> ruleBuilder) where T : class
    {
        /// <summary>
        /// Adds a validator to ensure the string matches the ISO 4217 currency code format (3 uppercase letters).
        /// Null values pass this rule; use <c>NotNull</c> or <c>NotEmpty</c> when the currency is required.
        /// </summary>
        /// <returns>The rule builder options for further configuration.</returns>
        public IRuleBuilderOptions<T, string> MustMatchesCurrencyFormat()
        {
            return ruleBuilder.Must(IsValid)
                .WithMessage("Currency must be 3 uppercase letters (A-Z).");
        }

        /// <summary>
        /// Validates that the property value is a valid currency value.
        /// </summary>
        /// <returns>The configured validation rule options.</returns>
        public IRuleBuilderOptionsConditions<T, string?> MustBeValidCurrency()
        {
            return ruleBuilder.Custom((value, context) =>
            {
                try
                {
                    _ = new Currency(value);
                }
                catch (Exception ex)
                {
                    string errorMessage = ex is BadRequestException
                        ? ex.Message
                        : $"Failed validation {context.PropertyPath}";

                    context.AddFailure(new ValidationFailure(context.PropertyPath, errorMessage));
                }
            });
        }
    }
}
