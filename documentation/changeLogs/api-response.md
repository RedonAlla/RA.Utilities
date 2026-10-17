---
title: RA.Utilities.Api.Response
authors: [RedonAlla]
date: 2026-10-17
---

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-17%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Response/10.1.0)

This release aligns the namespace with the package name, redesigns the constructor overloads around `static readonly` constants, and adopts the enum-based `ResponseType` serialization from `RA.Utilities.Core.Constants` v10.0.4.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **Namespace renamed to `RA.Utilities.Api.Response`**: All public types moved from `RA.Utilities.Api.Results` to `RA.Utilities.Api.Response`, matching the package id and removing the mismatch with the deprecated `RA.Utilities.Api.Results` package. **Migration**: Replace `using RA.Utilities.Api.Results;` with `using RA.Utilities.Api.Response;` and update any fully qualified names. See the [Migration Guide](/RA.Utilities/nuget-packages/api/RA.Utilities.Api.Response/migration-guides).
*   **Constructor overloads split**: Optional parameters that used `BaseResponseCode`/`BaseResponseMessages` values are replaced by convenience constructors plus all-argument overloads, because those constants are now `static readonly` (not compile-time constants). `BadRequestResponse` and `ConflictResponse` explicit overloads now require both a code and a message; `SuccessResponse<T>` gained `()`, `(int responseCode)`, and `(string responseMessage)` constructors. **Migration**: Prefer the single-payload convenience constructor; for custom values pass the code and message positionally (`responseCode: null` keeps the standard code on nullable overloads). Note that the all-argument overloads default `responseMessage` to `""`, not to the standard message.
*   **`responseType` is numeric by default**: This package now depends on `RA.Utilities.Core.Constants` v10.0.4, where `ResponseType` is a plain enum with no built-in converter. JSON output changes from `"responseType": "NotFound"` to `"responseType": 8`. **Migration**: Register `JsonStringEnumConverter` (`ConfigureHttpJsonOptions` for minimal APIs, `AddControllers().AddJsonOptions` for MVC) to keep string values, or update clients to accept the numeric contract.

### ✨ New Features

*   **Convenience constructors**: `BadRequestResponse(BadRequestResult)`, `BadRequestResponse(BadRequestResult[])`, `ConflictResponse(ConflictResult)`, `ErrorResponse(ErrorResult?)`, `ForbiddenResponse(ErrorResult?)`, and `NotFoundResponse(NotFoundResult)` apply the standard HTTP code, `ResponseType`, and message with one argument.
*   **Parameterless and message-only success responses**: `SuccessResponse<T>()`, `SuccessResponse<T>(int responseCode)`, and `SuccessResponse<T>(string responseMessage)` return a success envelope with `Result = default`.
*   **Nullable response codes**: The error overloads accept `int? responseCode`, falling back to the standard code when `null` is supplied.

### 📝 Improvements

*   **`UnprocessableResponse` null handling**: `UnprocessableResponse(ErrorResult)` and its explicit overload substitute a standard `ErrorResult` when the payload is `null`.
*   **XML documentation**: Constructor summaries now describe which values each overload applies.

---

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-16%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Response/10.0.0)

### ✨ New Features & Improvements

*   **Package revived**: The response models consolidated into `RA.Utilities.Api` in v10.0.6 are published again as the standalone `RA.Utilities.Api.Response` package.
The `RA.Utilities.Api.Results` namespace and all public types were unchanged.
*   `RA.Utilities.Api` now depends on this package, so existing consumers receive the types transitively; projects that only need response models can reference this package directly.

<!-- truncate -->