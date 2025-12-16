using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.SchemaTransformers;

/// <summary>
/// A custom schema transformer that enriches the OpenAPI schema with validation rules from FluentValidation.
/// This works by finding the corresponding FluentValidation validator for a given schema and applying the rules.
/// </summary>
internal sealed class FluentValidationSchemaTransformer(IServiceProvider serviceProvider) : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Type validatorType = typeof(IValidator<>).MakeGenericType(context.JsonTypeInfo.Type);

        if (serviceProvider.GetService(validatorType) is not IValidator validator)
        {
            return Task.CompletedTask;
        }

        IValidatorDescriptor descriptor = validator.CreateDescriptor();
        ExtractValidationRules(schema, context.JsonTypeInfo.Type, descriptor);
        return Task.CompletedTask;
    }

    private void ExtractValidationRules(OpenApiSchema schema, Type modelType, IValidatorDescriptor descriptor)
    {
        PropertyInfo[] properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        IDictionary<string, IOpenApiSchema>? schemaProperties = schema.Properties;

        if (schemaProperties == null || properties.Length == 0)
        {
            return;
        }

        foreach (PropertyInfo property in properties)
        {
            string propertyName = property.Name;
            string camelCasePropertyName = ToCamelCase(propertyName);

            if (!schemaProperties.TryGetValue(camelCasePropertyName, out IOpenApiSchema? propertySchema) ||
                propertySchema is not OpenApiSchema openApiPropertySchema)
            {
                continue;
            }

            IEnumerable<IValidationRule>? rules = descriptor.GetRulesForMember(propertyName);
            if (rules == null)
            {
                continue;
            }

            ApplyRulesToProperty(schema, openApiPropertySchema, camelCasePropertyName, rules);
        }
    }

    private void ApplyRulesToProperty(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string propertyName, IEnumerable<IValidationRule> rules)
    {
        foreach (IValidationRule rule in rules)
        {
            if (rule.Components == null)
            {
                continue;
            }

            foreach (IRuleComponent? component in rule.Components)
            {
                if (component is null)
                {
                    continue;
                }

                ApplyValidationRule(parentSchema, propertySchema, propertyName, component.Validator);
            }
        }
    }

    private void ApplyValidationRule(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string propertyName, IPropertyValidator validator)
    {
        switch (validator)
        {
            case INotNullValidator or INotEmptyValidator:
                MakePropertyRequired(parentSchema, propertyName);
                break;

            case ILengthValidator lengthValidator:
                if (lengthValidator.Min > 0)
                {
                    propertySchema.MinLength = lengthValidator.Min;
                }

                if (lengthValidator.Max is > 0 and < int.MaxValue)
                {
                    propertySchema.MaxLength = lengthValidator.Max;
                }

                break;

            case IRegularExpressionValidator regexValidator:
                propertySchema.Pattern = regexValidator.Expression;
                break;

            case IBetweenValidator betweenValidator when IsNumeric(propertySchema.Type):
                SetBetweenValidator(propertySchema, betweenValidator);
                break;

            case IComparisonValidator comparisonValidator when IsNumeric(propertySchema.Type) && comparisonValidator.ValueToCompare != null:
                SetComparisonValidator(propertySchema, comparisonValidator);
                break;

            case IEmailValidator:
                propertySchema.Format = "email";
                break;

            case ICreditCardValidator:
                propertySchema.Format = "credit-card";
                break;

            default:
                HandleOtherValidators(propertySchema, validator);
                break;
        }
    }

    private void SetBetweenValidator(OpenApiSchema propertySchema, IBetweenValidator betweenValidator)
    {
        try
        {
            decimal minValue = Convert.ToDecimal(betweenValidator.From, System.Globalization.CultureInfo.InvariantCulture);
            decimal maxValue = Convert.ToDecimal(betweenValidator.To, System.Globalization.CultureInfo.InvariantCulture);
            propertySchema.Minimum = minValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            propertySchema.Maximum = maxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string validatorTypeName = betweenValidator.GetType().Name;
            bool isExclusive = validatorTypeName.Contains("Exclusive", StringComparison.OrdinalIgnoreCase);
            propertySchema.ExclusiveMinimum = isExclusive.ToString(System.Globalization.CultureInfo.InvariantCulture);
            propertySchema.ExclusiveMaximum = isExclusive.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            // Ignore conversion errors
        }
    }

    private void SetComparisonValidator(OpenApiSchema propertySchema, IComparisonValidator comparisonValidator)
    {
        try
        {
            string value = Convert.ToDecimal(comparisonValidator.ValueToCompare, System.Globalization.CultureInfo.InvariantCulture)
                .ToString(System.Globalization.CultureInfo.InvariantCulture);

            switch (comparisonValidator.Comparison)
            {
                case Comparison.GreaterThan:
                    propertySchema.Minimum = value;
                    propertySchema.ExclusiveMinimum = true.ToString();
                    break;
                case Comparison.GreaterThanOrEqual:
                    propertySchema.Minimum = value;
                    propertySchema.ExclusiveMinimum = false.ToString();
                    break;
                case Comparison.LessThan:
                    propertySchema.Maximum = value;
                    propertySchema.ExclusiveMaximum = true.ToString();
                    break;
                case Comparison.LessThanOrEqual:
                    propertySchema.Maximum = value;
                    propertySchema.ExclusiveMaximum = false.ToString();
                    break;
            }
        }
        catch
        {
            // Ignore conversion errors
        }
    }

    private void HandleOtherValidators(OpenApiSchema propertySchema, IPropertyValidator validator)
    {
        Type validatorType = validator.GetType();
        string validatorName = validatorType.Name;

        if (validatorName.Contains("MinimumLength"))
        {
            PropertyInfo? lengthProperty = validatorType.GetProperty("Min") ?? validatorType.GetProperty("Length");

            if (lengthProperty?.GetValue(validator) is int intLength)
            {
                propertySchema.MinLength = intLength;
            }
        }
        else if (validatorName.Contains("MaximumLength"))
        {
            PropertyInfo? lengthProperty = validatorType.GetProperty("Max") ?? validatorType.GetProperty("Length");

            if (lengthProperty?.GetValue(validator) is int intLength)
            {
                propertySchema.MaxLength = intLength;
            }
        }
    }

    private void MakePropertyRequired(OpenApiSchema schema, string propertyName)
    {
        schema.Required ??= new HashSet<string>();
        schema.Required.Add(propertyName);
    }

    private string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
        {
            return name;
        }

        return char.ToLowerInvariant(name[0]) + name[1..];
    }

    private bool IsNumeric(JsonSchemaType? schemaType)
        => schemaType == JsonSchemaType.Integer || schemaType == JsonSchemaType.Number;
}
