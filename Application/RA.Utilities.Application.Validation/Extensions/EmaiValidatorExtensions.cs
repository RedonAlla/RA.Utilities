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
    /// <summary>
    /// Validates that the property value is a valid email address.
    /// </summary>
    /// <typeparam name="T">The model type containing the property.</typeparam>
    /// <param name="ruleBuilder">The FluentValidation rule builder.</param>
    /// <returns>The configured validation rule options.</returns>
    public static IRuleBuilderOptionsConditions<T, string?> MustBeValidEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
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
