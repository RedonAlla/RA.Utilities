---
title: HealthChecksDocumentTransformer
sidebar_position: 8
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `HealthChecksDocumentTransformer` is an `IOpenApiDocumentTransformer` that adds your service's health check endpoints to the OpenAPI document.

### 🎯 Purpose

Endpoints mapped with `MapHealthChecks` carry no API metadata, so ASP.NET Core's OpenAPI generator skips them and they never appear in the generated document.
For consumers, load balancers, and ops tooling that probe these endpoints, the API contract is incomplete without them.

This transformer documents the readiness endpoint (`/health`) with the JSON report schema written by `UIResponseWriter.WriteHealthCheckUIResponse` and the liveness endpoint (`/alive`) with its plain text response — no attributes or manual OpenAPI edits required.

### ⚙️ How It Works

1. **Documents `GET /health`**: Adds an operation (summary *"Service health"*) that returns the aggregate health status of the service together with the result of each registered health check, with `200` and `503` responses described as `application/json`.
2. **Documents `GET /alive`**: Adds an operation (summary *"Service liveness"*) that returns the liveness state of the service as plain text, based on the health checks tagged with `live`, with `200` and `503` responses described as `text/plain`.
3. **Adds component schemas**: `HealthCheckReport` (aggregate `status`, `totalDuration`, and per-check `entries`) and `HealthCheckEntry` (`data`, `description`, `duration`, `exception`, `status`, `tags`). The `status` property is an enum of `Healthy`, `Degraded`, and `Unhealthy`.
4. **Adds the `Health` tag** with a description, and applies it to both operations.
5. **Idempotent**: Paths, schemas, and tags already present in the document are left untouched.
6. **Stays anonymous**: Registered after `BearerSecurityDocumentTransformer`, so no security scheme is applied to the health check operations. They must stay reachable without authentication.

### 🚀 Usage

The transformer is registered automatically when you use the default set of transformers via [`AddDefaultsDocumentTransformer`](../Extensions/OpenApiOptionsExtensions.md#adddefaultsdocumenttransformer):

```csharp showLineNumbers
// Program.cs
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddOpenApi(options =>
{
    options.AddDefaultsDocumentTransformer();
});
// highlight-end

var app = builder.Build();

// JSON report for /health (the schema documented by the transformer)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // highlight-next-line
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Plain-text liveness state for /alive
app.MapHealthChecks("/alive", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});

app.MapOpenApi();

app.Run();
```

`UIResponseWriter.WriteHealthCheckUIResponse` comes from the `AspNetCore.HealthChecks.UI.Client` package; add it to your application if you want the documented JSON shape. The transformer itself only describes the contract — it does not write the health response.

With this setup, the generated document contains both health check operations under the `Health` tag, including their response schemas.

:::caution[v10.1.0 standalone extension, still present in v10.1.1]
`AddHealthChecksDocumentTransformer(IDictionary<string, string> tags)` currently registers `HeadersParameterTransformer` instead of `HealthChecksDocumentTransformer`, and the `tags` argument is ignored. Use `AddDefaultsDocumentTransformer()` until this is corrected.
:::
