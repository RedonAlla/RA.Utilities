using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.Models;

namespace RA.Utilities.OpenApi.OperationTransformers;

/// <summary>
/// Writes group-level documentation, declared with <c>WithGroupSummary</c> and
/// <c>WithGroupDescription</c> on a route group, into the OpenAPI tags of the group.
/// </summary>
/// <remarks>
/// OpenAPI has no notion of a group description: tags are the only place where a whole
/// group can be described. Operation-level <c>WithSummary</c>/<c>WithDescription</c> is left
/// untouched, so the group text and the endpoint text never fight over the same field.
/// </remarks>
internal sealed class GroupTagsDocumentTransformer : IOpenApiDocumentTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(context);

        foreach (ApiDescription api in context.DescriptionGroups.SelectMany(group => group.Items))
        {
            IList<object> metadata = api.ActionDescriptor.EndpointMetadata ?? [];
            GroupOpenApiMetadata? groupMetadata = GetGroupMetadata(metadata);

            if (groupMetadata is null)
                continue;

            string? description = ComposeDescription(groupMetadata);

            if (description is null)
                continue;

            IEnumerable<string> tagNames = metadata
                .OfType<ITagsMetadata>()
                .SelectMany(tags => tags.Tags)
                .Distinct(StringComparer.Ordinal);

            foreach (string tagName in tagNames)
                AddOrUpdateTag(document, tagName, description);
        }

        return Task.CompletedTask;
    }

    private static GroupOpenApiMetadata? GetGroupMetadata(IList<object> metadata)
    {
        string? summary = null;
        string? description = null;
        bool found = false;

        foreach (object item in metadata)
        {
            if (item is not GroupOpenApiMetadata group)
                continue;

            found = true;
            summary = group.Summary ?? summary;
            description = group.Description ?? description;
        }

        return found ? new GroupOpenApiMetadata(summary, description) : null;
    }

    private static string? ComposeDescription(GroupOpenApiMetadata metadata)
    {
        bool hasSummary = !string.IsNullOrWhiteSpace(metadata.Summary);
        bool hasDescription = !string.IsNullOrWhiteSpace(metadata.Description);

        if (hasSummary && hasDescription)
            return $"**{metadata.Summary}**\n\n{metadata.Description}";

        return hasSummary ? metadata.Summary : metadata.Description;
    }

    private static void AddOrUpdateTag(OpenApiDocument document, string tagName, string description)
    {
        document.Tags ??= new HashSet<OpenApiTag>();

        OpenApiTag? tag = document.Tags.FirstOrDefault(candidate => string.Equals(candidate.Name, tagName, StringComparison.Ordinal));
        if (tag is null)
        {
            document.Tags.Add(new OpenApiTag { Name = tagName, Description = description });
        }
        else if (string.IsNullOrWhiteSpace(tag.Description))
        {
            tag.Description = description;
        }
    }
}
