using System;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Application.Validation.Extensions;

/// <summary>
/// Validation extensions for IBAN rules.
/// </summary>
public static class IbanValidatorExtensions
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder) where T : class
    {
        /// <summary>
        /// Validates that the property value is a valid IBAN.
        /// </summary>
        /// <returns>The configured validation rule options.</returns>
        public IRuleBuilderOptionsConditions<T, string?> MustBeValidIban()
        {
            return ruleBuilder.Custom((value, context) =>
            {
                try
                {
                    _ = new IBAN(value);
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
