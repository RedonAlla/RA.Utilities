---
title: RouteGroupBuilderExtensions
sidebar_position: 7
---

```powershell
Namespace: RA.Utilities.OpenApi.Extensions
```

The `RouteGroupBuilderExtensions` class declares group-level OpenAPI documentation conventions on a [`RouteGroupBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routegroupbuilder). Unlike the endpoint-level `WithSummary()` / `WithDescription()` metadata, which is rendered per operation, these values describe the whole route group.

The methods store an internal `GroupOpenApiMetadata` in the route group's metadata. Every endpoint in the group — including endpoints in nested groups — inherits it, and the [`GroupTagsDocumentTransformer`](../OperationTransformers/GroupTagsDocumentTransformer.md) projects it onto the OpenAPI tags carried by those endpoints.

### Members

| Method | Returns | Description |
|---|---|---|
| `WithGroupSummary(string summary)` | `RouteGroupBuilder` | Sets the group summary, rendered as the bold heading of the tag description. |
| `WithGroupDescription(string description)` | `RouteGroupBuilder` | Sets the group description, rendered as the body of the tag description. |

Both methods throw `ArgumentNullException` when the route group is `null`. They do not validate the text itself; a `null` or whitespace-only value is ignored when the description is composed.

When the transformer finds more than one `GroupOpenApiMetadata` entry in an endpoint's metadata (for example, a parent group and a nested group), it merges the entries and the last non-null summary or description in metadata order wins.

### How the description is composed

| Summary set | Description set | Tag description |
|---|---|---|
| Yes | Yes | `**Summary**` + blank line + `Description` |
| Yes | No | `Summary` |
| No | Yes | `Description` |
| No | No | No tag is written |

### Usage

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    // highlight-next-line
    options.AddDefaultsDocumentTransformer(); // registers GroupTagsDocumentTransformer
});

var app = builder.Build();

var orders = app.MapGroup("/orders")
    // highlight-start
    .WithTags("Orders")
    .WithGroupSummary("Order management")
    .WithGroupDescription("Create, update, and track customer orders.");
    // highlight-end

orders.MapGet("/{id:int}", (int id) => $"Order {id}");

app.MapOpenApi();
app.Run();
```

Group documentation is written only to tags the group's endpoints actually carry, so call `.WithTags(...)` on the group (or on its endpoints). A tag description that is already present is never overwritten — see [`GroupTagsDocumentTransformer`](../OperationTransformers/GroupTagsDocumentTransformer.md) for the exact rules.
