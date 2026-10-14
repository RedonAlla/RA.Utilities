using System;
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Application.Validation.Extensions;

/// <summary>
/// Validation extensions for email address rules.
/// </summary>
public static class EmailValidatorExtensions
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder) where T : class
    {
        /// <summary>
        /// Validates that the property value is a valid email address.
        /// </summary>
        /// <returns>The configured validation rule options.</returns>
        public IRuleBuilderOptionsConditions<T, string?> MustBeValidEmail()
        {
            return ruleBuilder.Custom((value, context) =>
            {
                try
                {
                    _ = new Email(value);
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
