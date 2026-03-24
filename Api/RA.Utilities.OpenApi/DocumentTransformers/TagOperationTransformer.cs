using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// Transforms the OpenAPI document by adding or updating tag descriptions based on the provided configuration.
/// </summary>
internal sealed class TagOperationTransformer : IOpenApiDocumentTransformer
{
    private readonly IDictionary<string, string> _tagDetails;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagOperationTransformer"/> class.
    /// </summary>
    /// <param name="tags">A dictionary of tag names and their descriptions.</param>
    public TagOperationTransformer(IDictionary<string, string> tags)
    {
        _tagDetails = tags;
    }

    /// <summary>
    /// Transforms the OpenAPI document by adding or updating tags with the configured descriptions.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the transformation is finished.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (_tagDetails is null || _tagDetails.Count == 0)
        {
            return Task.CompletedTask;
        }

        document.Tags ??= new HashSet<OpenApiTag>();

        if (document.Tags.Count == 0)
        {
            foreach (KeyValuePair<string, string> tag in _tagDetails)
            {
                document.Tags.Add(new OpenApiTag { Name = tag.Key, Description = tag.Value });
            }

            return Task.CompletedTask;
        }

        var tagLookup = new Dictionary<string, OpenApiTag>(document.Tags.Count, StringComparer.Ordinal);

        foreach (OpenApiTag existingTag in document.Tags)
        {
            if (!string.IsNullOrEmpty(existingTag.Name))
            {
                tagLookup[existingTag.Name] = existingTag;
            }
        }

        foreach (KeyValuePair<string, string> tag in _tagDetails)
        {
            if (tagLookup.TryGetValue(tag.Key, out OpenApiTag? openApiTag))
            {
                openApiTag.Description = tag.Value;
            }
            else
            {
                document.Tags.Add(new OpenApiTag
                {
                    Name = tag.Key,
                    Description = tag.Value
                });
            }
        }

        return Task.CompletedTask;
    }
}
