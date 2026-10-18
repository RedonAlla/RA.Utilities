using System;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi;
using RA.Utilities.Core.ValueObjects;
using RA.Utilities.OpenApi.SchemaTransformers;

namespace RA.Utilities.Tests.RA.Utilities.OpenApi.SchemaTransformers;

/// <summary>
/// Contains unit tests for the <see cref="ValueObjectSchemaTransformer"/>.
/// </summary>
public class ValueObjectSchemaTransformerTests
{
    /// <summary>
    /// Provides the string-based value objects with the constraints the transformer should apply.
    /// </summary>
    public static TheoryData<Type, string?, string, int, int> StringValueObjects => new()
    {
        { typeof(Email), "email", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 1, 254 },
        { typeof(Currency), null, @"^[A-Z]{3}$", 3, 3 },
        { typeof(SSN), null, @"^[A-Z][0-9]{8}[A-Z]$", 10, 10 },
        { typeof(IBAN), "iban", @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{9,30}$", 13, 34 },
    };

    [Theory]
    [MemberData(nameof(StringValueObjects))]
    public void Apply_StringValueObject_ShouldSetStringConstraints(
        Type type,
        string? format,
        string pattern,
        int minLength,
        int maxLength)
    {
        // Arrange
        var schema = new OpenApiSchema();

        // Act
        ValueObjectSchemaTransformer.Apply(schema, type);

        // Assert
        schema.Type.Should().Be(JsonSchemaType.String);
        schema.Format.Should().Be(format);
        schema.Pattern.Should().Be(pattern);
        schema.MinLength.Should().Be(minLength);
        schema.MaxLength.Should().Be(maxLength);
        schema.Properties.Should().BeNull();
        schema.Description.Should().NotBeNullOrWhiteSpace();
        schema.Examples.Should().NotBeNull();
        schema.Examples.Should().ContainSingle();
        schema.Examples[0].GetValue<string>().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Apply_Money_ShouldSetObjectDescriptionAndExample()
    {
        // Arrange
        var schema = new OpenApiSchema();

        // Act
        ValueObjectSchemaTransformer.Apply(schema, typeof(Money));

        // Assert
        schema.Type.Should().Be(JsonSchemaType.Object);
        schema.Description.Should().Contain("monetary amount");
        schema.Examples.Should().NotBeNull();
        schema.Examples.Should().ContainSingle();

        JsonObject example = schema.Examples[0].AsObject();
        example["amount"]!.GetValue<decimal>().Should().Be(19.99m);
        example["currency"]!.GetValue<string>().Should().Be("EUR");
    }

    [Fact]
    public void Apply_UnknownType_ShouldLeaveSchemaUnchanged()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Integer,
            Description = "Existing description.",
        };

        // Act
        ValueObjectSchemaTransformer.Apply(schema, typeof(Guid));

        // Assert
        schema.Type.Should().Be(JsonSchemaType.Integer);
        schema.Description.Should().Be("Existing description.");
        schema.Examples.Should().BeNull();
    }
}
