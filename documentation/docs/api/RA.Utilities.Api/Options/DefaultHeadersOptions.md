---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Api.Middlewares.Options
```

The `DefaultHeadersOptions` class provides configuration for the [`DefaultHeadersMiddleware`](../DefaultHeadersMiddleware). It allows you to specify request paths that should be excluded from header enforcement.

## Properties

| Property | Type | Description |
| -------- | ---- | ----------- |
| **PathsToIgnore** | `ISet<string>` | A set of request path prefixes to ignore for header enforcement. Paths starting with any value in this set will skip validation. Comparisons are case-insensitive. |

## Usage

Configure `DefaultHeadersOptions` via the `AddDefaultHeadersMiddleware()` extension method:

```csharp
using RA.Utilities.Api.Middlewares.Extensions;

builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
});
```

With this configuration, requests to `/swagger`, `/swagger/index.html`, `/health`, `/health/detailed`, etc. will bypass header validation.
