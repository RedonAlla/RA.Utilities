using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.OpenApi.Models;

namespace RA.Utilities.OpenApi.Extensions;

/// <summary>
/// Group-level OpenAPI documentation conventions. Unlike the endpoint-level
/// <c>WithSummary</c>/<c>WithDescription</c> metadata, which is rendered per operation,
/// these values describe the whole route group.
/// </summary>
public static class RouteGroupBuilderExtensions
{
    extension(RouteGroupBuilder group)
    {
        /// <summary>
        /// Documents the group with a short summary. Rendered as the heading of the group description.
        /// </summary>
        /// <param name="summary">The group summary.</param>
        /// <returns>The route group for chaining.</returns>
        public RouteGroupBuilder WithGroupSummary(string summary)
        {
            ArgumentNullException.ThrowIfNull(group);
            return group.WithMetadata(new GroupOpenApiMetadata(Summary: summary));
        }

        /// <summary>
        /// Documents the group with a description. Rendered as the body of the group description.
        /// </summary>
        /// <param name="description">The group description.</param>
        /// <returns>The route group for chaining.</returns>
        public RouteGroupBuilder WithGroupDescription(string description)
        {
            ArgumentNullException.ThrowIfNull(group);
            return group.WithMetadata(new GroupOpenApiMetadata(Description: description));
        }
    }
}
