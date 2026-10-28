---
title: GroupTagsDocumentTransformer
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.OpenApi.OperationTransformers
```

The `GroupTagsDocumentTransformer` writes group-level documentation — declared with [`WithGroupSummary()` and `WithGroupDescription()`](../Extensions/RouteGroupBuilderExtensions.md) on a route group — into the OpenAPI tags of that group.

:::info[Namespace]
Although the class lives in the `RA.Utilities.OpenApi.OperationTransformers` namespace, it implements `IOpenApiDocumentTransformer` and runs once against the whole document. All other types in this namespace are operation transformers.
:::

### 🎯 Purpose

OpenAPI has no notion of a group description: tags are the only place where a whole group can be described. Endpoint-level `WithSummary()` / `WithDescription()` is left untouched, so the group text and the endpoint text never fight over the same field.

### ⚙️ How It Works

1. **Scans endpoints**: Iterates every `ApiDescription` in the document's description groups.
2. **Reads group metadata**: Collects the internal `GroupOpenApiMetadata` entries from each endpoint's metadata (route-group conventions propagate them to every endpoint in the group). Entries are merged, and the last non-null summary/description in metadata order wins.
3. **Composes the text**: `Summary` and `Description` together become `**Summary**` + a blank line + the description; if only one is set, it is used alone. If both are blank, the endpoint is skipped.
4. **Writes document tags**: Collects the distinct tag names from the endpoints' `ITagsMetadata` (the tags added with `WithTags(...)`) and adds each tag to `document.Tags` — or, if the tag already exists, fills its `Description` only when that description is empty.

### 🚀 Usage

The transformer is registered automatically by [`AddDefaultsDocumentTransformer()`](../Extensions/OpenApiOptionsExtensions.md#adddefaultsdocumenttransformer):

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;

builder.Services.AddOpenApi(options =>
{
    options.AddDefaultsDocumentTransformer();
});

var orders = app.MapGroup("/orders")
    .WithTags("Orders")
    .WithGroupSummary("Order management")
    .WithGroupDescription("Create, update, and track customer orders.");
```

To register it on its own, use [`AddGroupTagsDocumentTransformer`](../Extensions/OpenApiOptionsExtensions.md#addgrouptagsdocumenttransformeridictionarystring-string-tags):

```csharp showLineNumbers
builder.Services.AddOpenApi(options =>
{
    // The argument is required by the current signature but is not used.
    options.AddGroupTagsDocumentTransformer(new Dictionary<string, string>());
});
```

### Behavior notes

- **Tags are required**: Group text is written to the tags the group's endpoints carry. A group without `.WithTags(...)` (and whose endpoints have no other tag metadata) produces no tag description.
- **Existing descriptions win**: A tag whose description is already non-empty is left untouched — for example one configured with `AddTagOperationTransformer()` or emitted by another transformer. Group documentation only fills empty descriptions.
- **Every tag on the group receives the text**: If the group's endpoints carry several tags, the composed description is applied to each of them.
- **Idempotent**: Running the transformer more than once produces the same tags.
