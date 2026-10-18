using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.OpenApi.SchemaTransformers;

/// <summary>
/// Enriches the OpenAPI schemas of the value objects from <c>RA.Utilities.Core.ValueObjects</c>
/// with their validation constraints, examples, and descriptions.
/// </summary>
/// <remarks>
/// The OpenAPI generator cannot infer the constraints enforced by a value object's constructor,
/// so it documents <see cref="Email"/>, <see cref="Currency"/>, <see cref="SSN"/>, and
/// <see cref="IBAN"/> as property-less objects. This transformer replaces those schemas with
/// string schemas that carry the same rules the value objects enforce, and adds an example to
/// the <see cref="Money"/> schema. It also applies the definitions to route, query, and header
/// parameters, which are bound through <see cref="IParsable{TSelf}"/> and are reported as strings
/// by the OpenAPI generator.
/// </remarks>
internal sealed class ValueObjectSchemaTransformer : IOpenApiSchemaTransformer
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private const string CurrencyPattern = @"^[A-Z]{3}$";
    private const string SsnPattern = @"^[A-Z][0-9]{8}[A-Z]$";
    private const string IbanPattern = @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{9,30}$";

    private static readonly Dictionary<Type, Action<OpenApiSchema>> Definitions = new()
    {
        [typeof(Email)] = schema => ApplyStringSchema(
            schema,
            format: "email",
            pattern: EmailPattern,
            minLength: 1,
            maxLength: 254,
            example: "user@example.com",
            description: """
                A validated and normalized email address.
                Trimmed and lower-cased; must not exceed 254 characters and must match the format name@example.com.
                """
        ),
        [typeof(Currency)] = schema => ApplyStringSchema(
            schema,
            pattern: CurrencyPattern,
            minLength: 3,
            maxLength: 3,
            example: "EUR",
            description: """
            A validated and normalized ISO 4217 currency code.
            Trimmed and upper-cased; must be exactly 3 characters (for example, EUR).
            """
        ),
        [typeof(SSN)] = schema => ApplyStringSchema(
            schema,
            pattern: SsnPattern,
            minLength: 10,
            maxLength: 10,
            example: "J01234567R",
            description: """
                A validated and normalized social security number.
                Trimmed and upper-cased; must be exactly 10 characters in the format letter, eight digits, letter (for example, J01234567R).
            """
            ),
        [typeof(IBAN)] = schema => ApplyStringSchema(
            schema,
            format: "iban",
            pattern: IbanPattern,
            minLength: 13,
            maxLength: 34,
            example: "GB82WEST12345698765432",
            description: """
            A validated and normalized International Bank Account Number (ISO 13616).
            Trimmed, stripped of grouping separators and upper-cased; must be 13-34 characters and pass the ISO 7064 mod-97 check digit verification (for example, GB82WEST12345698765432).
            """),
        [typeof(Money)] = ApplyMoneySchema,
    };

    /// <summary>
    /// Applies the value object definition that matches <see cref="OpenApiSchemaTransformerContext.JsonTypeInfo"/>,
    /// if one is registered.
    /// </summary>
    /// <param name="schema">The OpenAPI schema to transform.</param>
    /// <param name="context">The schema transformer context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Type type = context.JsonTypeInfo.Type;

        // Route, query, and header parameters bind through IParsable<T>, so the JSON type is
        // reported as string and the actual value object type is only available on the parameter.
        if (!Definitions.ContainsKey(type) && context.ParameterDescription?.Type is Type parameterType)
        {
            type = parameterType;
        }

        Apply(schema, type);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Applies the definition registered for <paramref name="type"/> to <paramref name="schema"/>.
    /// </summary>
    /// <param name="schema">The schema to enrich.</param>
    /// <param name="type">The value object type to look up.</param>
    internal static void Apply(OpenApiSchema schema, Type type)
    {
        if (Definitions.TryGetValue(type, out Action<OpenApiSchema>? applyDefinition))
        {
            applyDefinition(schema);
        }
    }

    /// <summary>
    /// Rewrites a schema as a string schema with the supplied constraints.
    /// </summary>
    private static void ApplyStringSchema(
        OpenApiSchema schema,
        string description,
        string example,
        string? format = null,
        string? pattern = null,
        int? minLength = null,
        int? maxLength = null)
    {
        // Discard any object shape the generator may have inferred for the value object.
        schema.Properties = null;
        schema.Required = null;
        schema.Items = null;
        schema.AllOf = null;
        schema.OneOf = null;
        schema.AnyOf = null;
        schema.Enum = null;

        schema.Type = JsonSchemaType.String;
        schema.Format = format;
        schema.Pattern = pattern;
        schema.MinLength = minLength;
        schema.MaxLength = maxLength;
        schema.Description = description;
        schema.Examples = [JsonValue.Create(example)];
    }

    /// <summary>
    /// Adds a description and an example to the <see cref="Money"/> object schema.
    /// </summary>
    private static void ApplyMoneySchema(OpenApiSchema schema)
    {
        schema.Type = JsonSchemaType.Object;
        schema.Description = """
        A monetary amount in a specific currency.
        Use the PositiveMoney factory to reject negative amounts, and Add/Subtract to keep amounts in the same currency.
        """;
        schema.Examples =
        [
            new JsonObject
            {
                ["amount"] = 19.99m,
                ["currency"] = "EUR",
            },
        ];
    }
}
