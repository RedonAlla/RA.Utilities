using System.Text.RegularExpressions;
using FluentValidation;

namespace RA.Utilities.Application.Validation.Extensions;

/// <summary>
/// Provides extension methods for validating currency codes using FluentValidation.
/// </summary>
public static partial class CurrencyValidatorExtensions
{
    /// <summary>
    /// Compiled regex for currency validation (ISO 4217: 3 uppercase letters).
    /// </summary>
    [GeneratedRegex("^[A-Z]{3}$", RegexOptions.Compiled)]
    private static partial Regex CurrencyRegex();
    /// <summary>
    /// Adds a validator to ensure the string matches the ISO 4217 currency code format (3 uppercase letters).
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder for the string property.</param>
    /// <returns>The rule builder options for further configuration.</returns>
    public static IRuleBuilderOptions<T, string> MustMatchesCurrencyFormat<T>(this IRuleBuilder<T, string> ruleBuilder)
        where T : class
    {
        return ruleBuilder.Must(IsValid)
            .WithMessage("Currency must be 3 uppercase letters (A-Z).");
    }
    private static bool IsValid(string value) =>
        value != null && CurrencyRegex().IsMatch(value);
}
