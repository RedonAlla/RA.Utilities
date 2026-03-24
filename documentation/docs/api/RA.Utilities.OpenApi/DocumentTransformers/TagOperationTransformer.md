---
title: TagOperationTransformer
sidebar_position: 7
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The purpose of the `TagOperationTransformer` class is to **enrich the OpenAPI documentation by adding or updating descriptions for "Tags".**

Here is a detailed breakdown of what it does:

#### 1. Centralized Documentation:
In OpenAP/SwaggerI, "Tags" are typically used to group operations (often mapping to Controller names in ASP.NET Core).
By default, these tags might just be the controller name with no description.
`TagOperationTransformer` allows you to inject descriptions for these groups from a central configuration (a `Dictionary<string, string>`) rather than adding attributes to every single controller class or minimal api.

#### 2. Adds Missing Tags:
If your configuration includes a tag that isn't currently in the document (perhaps for a controller that hasn't been scanned yet or a manually defined tag), this transformer adds it to the global list of tags.

#### 3. Updates Existing Tags:
If the OpenAPI generator has already discovered tags (e.g., from your Controller names), `TagOperationTransformer` looks them up by name and updates their ***Description*** property with the value you provided.

## 🚀 Example Scenario:
If you have a `ProductsController`, Swagger usually generates a "Products" tag.

* **Without this transformer**: The "Products" section in Swagger UI has no description next to the header.
* **With this transformer**: You can map "Products" to "Operations related to inventory management", and that text will appear next to the group header in the UI.

## 🚀 Usage

You can register the `TagOperationTransformer` in your `Program.cs` file using the [`AddTagOperationTransformer`](../Extensions/DependencyInjectionExtensions.md#addtagoperationtransformeridictionarystring-string-tags) extension method.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    // highlight-start
    options.AddTagOperationTransformer(new Dictionary<string, string>
    {
        { "Products", "Operations related to product management." },
        { "Orders", "Operations for processing customer orders." },
        { "Auth", "Authentication and authorization endpoints." }
    });
    // highlight-end
});

var app = builder.Build();

app.MapOpenApi();

app.Run();
```