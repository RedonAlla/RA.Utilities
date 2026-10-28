using System;
using RA.Utilities.OpenApi.OperationTransformers;

namespace RA.Utilities.OpenApi.Models;

/// <summary>
/// Documentation metadata declared on a route group. Endpoints inherit it, and the
/// <see cref="GroupTagsDocumentTransformer"/> projects it onto the
/// OpenAPI tag that represents the group.
/// </summary>
/// <param name="Summary">Short summary of the group, rendered as the tag description heading.</param>
/// <param name="Description">Description of the group, rendered as the tag description.</param>
internal sealed record GroupOpenApiMetadata(string? Summary = null, string? Description = null);
