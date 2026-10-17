---
title: RA.Utilities.Core.Constants
authors: [RedonAlla]
date: 2026-10-17
---

## Version 10.0.4
![Date Badge](https://img.shields.io/badge/Publish-17%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.4-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.4)

`ResponseType` has been converted from an extensible `record` back to a plain C# `enum`, and every constant field has moved from `const` to `static readonly`. The values themselves are unchanged.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **`ResponseType` is now an `enum`, not a `record`**: The type-safe enum `record` introduced in v10.0.2 is replaced by a standard C# `enum` with the same built-in values (`Success`, `Created`, `Updated`, `Deleted`, `NoContent`, `Accepted`, `Validation`, `Problem`, `NotFound`, `Conflict`, `Unauthorized`, `Error`, `BadRequest`, `Unprocessable`, `Forbidden`, `TooManyRequests`, `ServiceUnavailable`, `GatewayTimeout`). The `Value` property, the implicit `string` conversion, and inheritance-based extensibility are removed. The built-in `ResponseTypeJsonConverter` is gone: with default `System.Text.Json` options the enum is written as a number (`NotFound` → `8`); register `JsonStringEnumConverter` to keep string output. **Migration**: Replace `responseType.Value` with `responseType.ToString()`; replace custom types derived from `ResponseType` with the closest built-in member; switch expressions now support constant patterns (`ResponseType.NotFound =>`); decide whether clients receive numbers (default) or names (converter). See the [Migration Guide](/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/migration-guides) for detailed before/after examples.
*   **Constant fields are now `static readonly`**: Every field in `BaseResponseCode`, `BaseResponseMessages`, `BaseErrorCode`, `BaseErrorMessage`, and `HeaderParameters` is `public static readonly` instead of `public const`, and the holders are `partial class` declarations instead of `static class`. Ordinary reads are unchanged, but the fields can no longer be used where C# requires a constant expression — attribute arguments, default parameter values, `case` labels, or `const` declarations. **Migration**: Use `static readonly` for locals/fields that reference these values, a nullable default plus `??` for optional parameters, and `if` or a local `const` where the compiler requires a constant. See the [Migration Guide](/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/migration-guides) for before/after examples.

### 📝 Notes

*   `ResponseType` uses `System.Text.Json` defaults: numeric wire values unless `JsonStringEnumConverter` is registered. With the converter, names are matched case-insensitively and unknown names (or `null` for a non-nullable enum) throw `JsonException`.
*   `default(ResponseType)` is `ResponseType.Success` (value `0`).
*   Because the fields are `static readonly`, correcting a value no longer requires recompiling consumers.



## Version 10.0.3
![Date Badge](https://img.shields.io/badge/Publish-22%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.3)

This release introduces a shared vocabulary for field-level validation failures: `BaseErrorCode` for machine-readable error codes and `BaseErrorMessage` for the matching human-readable messages.

<!-- truncate -->

### ✨ New Features

*   **`BaseErrorCode`**: New constants class providing stable, machine-readable error codes for validation failures around email (`REQUIRED_EMAIL`, `EMAIL_MAX_LENGTH`, `INVALID_EMAIL`), currency (`REQUIRED_CURRENCY`, `CURRENCY_LENGTH`, `CURRENCY_MISMATCH`), money (`POSITIVE_MONEY`), and SSN (`REQUIRED_SSN`, `SSN_LENGTH`, `NOT_VALID_SSN`). API consumers can branch on these codes programmatically without parsing message text.
*   **`BaseErrorMessage`**: New constants class with the matching human-readable messages (e.g., `The 'Email address' field is required.`), ensuring consistent wording for validation failures across all services.
*   **Value object integration**: These constants back the structured `ValidationError` entries thrown by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`) and are translated into standardized HTTP 400 responses by the `RA.Utilities.Api` middleware.

### 📝 Notes

*   `BaseErrorCode.EmailMaxLength` intentionally has no `BaseErrorMessage` counterpart — the maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text.



## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-27%20July%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.2)

This release delivers a significant architectural improvement to `ResponseType` while expanding the constant catalog with gateway timeout support.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **`ResponseType` is now a `record`, not an `enum`**: The `ResponseType` type has been converted from a traditional C# `enum` to a **type-safe enum pattern** using a `record`. This enables consuming projects to extend `ResponseType` with custom values via inheritance. The built-in values (`Success`, `Validation`, `Problem`, `NotFound`, `Conflict`, `Unauthorized`, `Error`, `BadRequest`, `Unprocessable`, `Forbidden`, `GatewayTimeout`) remain available as `public static readonly` fields. A custom `ResponseTypeJsonConverter` handles JSON serialization to/from plain strings. **Migration**: Replace any `ResponseType.SomeValue` usage in switch expressions or enum-specific APIs with the equivalent record field access; serialization behavior is unchanged. See the [Migration Guide](/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/migration-guides) for detailed before/after examples.

### ✨ New Features

*   **`BaseResponseCode.GatewayTimeout`**: Added `GatewayTimeout = 504` constant for HTTP 504 Gateway Timeout responses.
*   **`BaseResponseMessages.GatewayTimeout`**: Added a default message for gateway timeout scenarios.
*   **`ResponseType.GatewayTimeout`**: Added a new built-in response type field for gateway timeout outcomes.

### 📝 Improvements

*   **XML Documentation**: All XML doc comments in `BaseResponseCode`, `BaseResponseMessages`, and `ResponseType` have been revised for consistency and clarity, explicitly referencing the HTTP status code each constant represents.

---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-24%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.1)

This release focuses on maintaining consistency within the RA.Utilities ecosystem and refining documentation for core constant values.

### ✨ Improvements

*   **Alignment**: Version alignment with `RA.Utilities.Core.Exceptions` to support a coordinated release of core building blocks.
*   **Documentation**: Updated the constants documentation to ensure clear mapping for the newly introduced semantic exceptions (Conflict/Forbidden).



## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.0)

Change the project version from `10.0.100-rc.2` to `10.0.0` to indicate the transition from release candidate to a stable version.
This marks the readiness of the RA Core Constants package for general availability in the RA Utilities ecosystem.

<!-- truncate -->

## Version 10.0.0-rc.2

![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.0-rc.2)


This release focuses on enhancing the clarity, consistency, and completeness of the constants provided by the package. The changes make the constants more intuitive to use and align the code with the documentation.


### ✨ Key Features

*   **Expanded `BaseResponseMessages`**:
    *   Added new constants for `Created`, `Updated`, `Deleted`, `Forbidden`, and `Conflict` to provide a more comprehensive set of standard messages.
    *   Improved the wording of existing messages for better clarity (e.g., `Success`, `BadRequest`, `NotFound`).
*   **Refined `ResponseType` Enum**:
    *   Removed the `Database` member to abstract away implementation details from the API contract, promoting a cleaner separation of concerns.
    *   Removed the redundant `Unknown` member, as `Error` serves as a better general-purpose error type.
*   **Improved Documentation**:
    *   The `README.md` has been significantly updated to accurately reflect all available constants in `BaseResponseCode`, `BaseResponseMessages`, `HeaderParameters`, and the `ResponseType` enum.
    *   Added clear tables and usage examples to improve the developer experience.

### 📝 Notes

The goal of this update is to make the `RA.Utilities.Core.Constants` package a more robust and self-documenting source of truth for your application's core values. These changes ensure that developers have a consistent and predictable set of constants for building API responses and handling HTTP headers.

---

Thank you for using RA.Utilities!