using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// Provides a filter for transforming OpenAPI documents to support polymorphic schemas.
/// This filter adds derived types and configures the discriminator for polymorphic base schemas.
/// </summary>
public class PolymorphismSchemaFilter : IOpenApiDocumentTransformer
{
    private readonly string _polymorphismPropertyName;
    private readonly string _discriminatorPropertyName;
    private readonly Dictionary<string, Type> _typesToInclude;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphismSchemaFilter"/> class.
    /// </summary>
    /// <param name="polymorphismPropertyName">The name of the property used for polymorphism in the schema.</param>
    /// <param name="typesToInclude">A dictionary mapping schema names to their corresponding types to include in the polymorphic schema.</param>
    /// <param name="discriminatorPropertyName">The name of the discriminator property. Defaults to "Type".</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="polymorphismPropertyName"/>, <paramref name="discriminatorPropertyName"/>, or <paramref name="typesToInclude"/> is <c>null</c>.
    /// </exception>
    public PolymorphismSchemaFilter(string polymorphismPropertyName, Dictionary<string, Type> typesToInclude, string discriminatorPropertyName = "Type")
    {
        ArgumentNullException.ThrowIfNull(polymorphismPropertyName);
        ArgumentNullException.ThrowIfNull(discriminatorPropertyName);
        ArgumentNullException.ThrowIfNull(typesToInclude);
        _polymorphismPropertyName = polymorphismPropertyName;
        _discriminatorPropertyName = discriminatorPropertyName;
        _typesToInclude = typesToInclude;
    }

    /// <summary>
    /// Transforms the OpenAPI document to support polymorphic schemas by adding derived types and configuring the discriminator.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (document?.Components?.Schemas == null)
        {
            return;
        }

        if (!(TryGetSchema(document.Components, _polymorphismPropertyName, out IOpenApiSchema baseSchema) && baseSchema is OpenApiSchema baseOpenApiSchema))
        {
            return;
        }

        IDictionary<string, IOpenApiSchema> schemas = document.Components.Schemas;

        foreach (KeyValuePair<string, Type> type in _typesToInclude)
        {
            // 1. Check if the schema already exists
            if (!schemas.ContainsKey(type.Key))
            {
                // Use the first available parameter description for schema generation
                ApiParameterDescription? paramDesc = context.DescriptionGroups.Count > 0 && context.DescriptionGroups[0].Items.Count > 0
                    ? context.DescriptionGroups[0].Items[0].ParameterDescriptions[0]
                    : null;

                if (paramDesc != null)
                {
                    OpenApiSchema newSchema = await context.GetOrCreateSchemaAsync(
                        type.Value,
                        paramDesc,
                        cancellationToken
                    ).ConfigureAwait(false);
                    schemas.Add(type.Key, newSchema);
                }
            }
        }

        string[] typeKeys = new string[_typesToInclude.Count];
        _typesToInclude.Keys.CopyTo(typeKeys, 0);

        // 4. Update the base schema to use oneOf with discriminator
        baseOpenApiSchema.Discriminator = new OpenApiDiscriminator
        {
            PropertyName = _discriminatorPropertyName,
            Mapping = DiscriminatorMapping(typeKeys)
        };

        baseOpenApiSchema.OneOf = OneOfList(typeKeys);
    }

    /// <summary>
    /// Attempts to retrieve a schema with the specified name from the given <see cref="OpenApiComponents"/>.
    /// </summary>
    /// <param name="components">The OpenAPI components to search.</param>
    /// <param name="schemaName">The name of the schema to retrieve.</param>
    /// <param name="schema">When this method returns, contains the schema if found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the schema was found; otherwise, <c>false</c>.</returns>
    private bool TryGetSchema(OpenApiComponents? components, string schemaName, out IOpenApiSchema? schema)
    {
        schema = null;
        return components?.Schemas != null && components.Schemas.TryGetValue(schemaName, out schema);
    }

    private Dictionary<string, OpenApiSchemaReference> DiscriminatorMapping(string[] types)
    {
        var mapping = new Dictionary<string, OpenApiSchemaReference>(types.Length);
        for (int i = 0; i < types.Length; i++)
        {
            mapping[types[i]] = new OpenApiSchemaReference(types[i]);
        }
        return mapping;
    }

    private List<IOpenApiSchema> OneOfList(string[] types)
    {
        var oneOfList = new List<IOpenApiSchema>(types.Length);
        for (int i = 0; i < types.Length; i++)
        {
            oneOfList.Add(new OpenApiSchemaReference(types[i]));
        }
        return oneOfList;
    }
}
