---
title: RA.Utilities.OpenApi
authors: [RedonAlla]
date: 2026-10-19
---

## Version 10.2.1
![Date Badge](https://img.shields.io/badge/Publish-19%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.2.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.2.1)

Dependency updates only — no API or behavior changes.

<!-- truncate -->

### 📝 Improvements

* **Microsoft.AspNetCore.OpenApi 10.0.12 → 10.0.13**.
* **Microsoft.OpenApi 2.12.2 → 2.13.0** (stays on the 2.x line required by `Microsoft.AspNetCore.OpenApi` 10.x).
* **FluentValidation 12.1.1 → 12.2.0**.

## Version 10.2.0
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.2.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.2.0)

The new `ValueObjectSchemaTransformer` documents the validation constraints of the value objects from `RA.Utilities.Core.ValueObjects`, and `AddDefaultsDocumentTransformer()` now registers it automatically.

<!-- truncate -->

`DocumentInfoTransformer` now populates the document `Summary`, and the registration extension class is renamed to `OpenApiOptionsExtensions` with its methods refactored to C# 14 extension members.

### ✨ New Features

* **Added `ValueObjectSchemaTransformer`**: The OpenAPI generator cannot see the validation performed by a value object's constructor, so `Email`, `Currency`, `SSN`, and `IBAN` were emitted as property-less objects. The transformer now documents their constraints:
    * `Email` — `string`, `format: email`, pattern, `minLength: 1`, `maxLength: 254`.
    * `Currency` — `string`, pattern, `minLength: 3`, `maxLength: 3`.
    * `SSN` — `string`, pattern, `minLength: 10`, `maxLength: 10`.
    * `IBAN` — `string`, `format: iban`, pattern, `minLength: 13`, `maxLength: 34`.
    * `Money` — object description and example.
    * Every schema also receives a description and an example value.
* **Added `AddValueObjectSchemaTransformer()`** for registering the transformer standalone.

### ⚠️ Breaking Change

* **`DependencyInjectionExtensions` renamed to `OpenApiOptionsExtensions`**: the static class holding all registration extension methods was renamed, and its methods are now declared as C# 14 extension members on `OpenApiOptions`. The namespace (`RA.Utilities.OpenApi.Extensions`), method names, signatures, and extension-method call sites are unchanged — only class-name references break (`typeof(DependencyInjectionExtensions)`, static calls, reflection).

### 📝 Improvements

* **`AddDefaultsDocumentTransformer()` now includes the value object schema transformer**, so the default setup documents value objects without additional configuration.
* **Added a dependency on `RA.Utilities.Core.ValueObjects` 10.2.0.**
* **Response model dependency updated**: the package now references `RA.Utilities.Api.Response` 10.1.0, whose models moved from the `RA.Utilities.Api.Results` namespace to `RA.Utilities.Api.Response`. `DefaultResponsesOperationTransformer` resolves `ErrorResponse` from the new namespace. The OpenApi public API is unchanged; update `using` directives only if your custom transformers reference those models.

### 📝 Notes

* The transformer is idempotent and is safe to register more than once.
* The release is backward compatible; existing OpenAPI output is unchanged except for the enriched value object schemas.

---

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-11%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.1.0)

### ✨ New Features

* **Added `HealthChecksDocumentTransformer`**: Documents the health check endpoints in the OpenAPI document. Endpoints mapped by `MapHealthChecks` carry no API metadata, so the OpenAPI generator never includes them; this transformer adds them explicitly.
    * Adds `GET /health` — the aggregate health status of the service together with the result of each registered health check, returned as `application/json` with `200` and `503` responses.
    * Adds `GET /alive` — the liveness state as plain text (`Healthy`, `Degraded` or `Unhealthy`), based on the health checks tagged with `live`, returned as `text/plain` with `200` and `503` responses.
    * Adds the `HealthCheckReport` and `HealthCheckEntry` component schemas and a `Health` tag with a description.
    * Registration is idempotent — paths, schemas and tags already present in the document are left untouched.
* **`AddDefaultsDocumentTransformer()` now includes the health checks transformer**, registered after `BearerSecurityDocumentTransformer` so that no security scheme is applied to the health check operations. They must stay reachable without authentication.
* **Added the `AddHealthChecksDocumentTransformer(IDictionary<string, string> tags)` extension method** for registering the transformer standalone.

### ⚠️ Known Issue

`AddHealthChecksDocumentTransformer(tags)` currently registers `HeadersParameterTransformer` instead of `HealthChecksDocumentTransformer`, and the `tags` argument is ignored. Use `AddDefaultsDocumentTransformer()` to document the health check endpoints until this is corrected.

<!-- truncate -->

## Version 10.0.11
![Date Badge](https://img.shields.io/badge/Publish-07%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.11-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.11)

### 🔧 Fixes & Improvements

* **Fixed `AddDefaultsDocumentTransformer` null handling**: Now throws `ArgumentNullException` immediately instead of silently skipping all transformers when options are null.
* **Fixed `DefaultResponsesOperationTransformer` duplicate guard**: The dedup check now correctly uses the response key `"500"` instead of `nameof(Response<>)`, preventing the 500 response from being added on every invocation.
* **Fixed `OpenApiOperationUtilities.AddResponseExample` null reference**: Replaced `operation.Responses!` with `operation.Responses?` to handle null responses safely.
* **Fixed `FluentValidationSchemaTransformer` boolean formatting**: `ExclusiveMinimum`/`ExclusiveMaximum` now use lowercase `"true"`/`"false"` string literals instead of culture-dependent `bool.ToString()`.
* **Fixed `EnumXmlSchemaTransformer` re-lock bug**: When the XML file doesn't exist, the null result is now cached to avoid re-entering the lock on every call.
* **Fixed `PolymorphismDocumentTransformer` array construction**: Simplified verbose `new[]` + `CopyTo` to a collection expression.
* **Fixed `HeadersParameterSettings` non-deterministic defaults**: Replaced `Guid.NewGuid()` with a fixed example GUID for consistent documentation snapshots.
* **Fixed filename typo**: `DepenenceInjectionExtensions.cs` → `DependencyInjectionExtensions.cs`.
* **Removed commented-out code** from `DocumentInfoTransformer`.
* **Added null guard** to `TagOperationTransformer` constructor.
* **Added defaults** to `OpenApiExampleModel` non-nullable properties.
* **Renamed `OpenApiResponseExample.StatusCodes`** → `StatusCode` (single int, not a collection).
* **Normalized `.csproj` indentation** to 2-space.
* **Added `InternalsVisibleTo`** for the test project.

### ⚠️ Breaking Change

`OpenApiResponseExample.StatusCodes` → `OpenApiResponseExample.StatusCode`. Update any code that sets or reads the old property name.

## Version 10.0.8
![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.8-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.8)

Updated `OpenApiOperationUtilities` to use `JsonNamingPolicy.CamelCase` when serializing OpenAPI examples.

## Version 10.0.7
![Date Badge](https://img.shields.io/badge/Publish-24%20March%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.7-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.7)

Introduced `TagOperationTransformer` for adding tag descriptions, plus performance optimizations.
