---
title: RA.Utilities.Core.Exceptions
authors: [RedonAlla]
date: 2026-10-18
---

## Version 10.1.0

![Date Badge](https://img.shields.io/badge/Publish-18%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.1.0)

This release adapts the exception hierarchy to the `static readonly` fields in `RA.Utilities.Core.Constants` v10.0.4+, adds convenience constructors for the most common shapes, and makes the default-message behavior explicit. There are no breaking changes for call sites.

<!-- truncate -->

### ✨ New Features

*   **Parameterless `NotFoundException` and `ConflictException`**: `new NotFoundException()` and `new ConflictException()` produce 404 and 409 responses with the standard package messages, so a generic failure no longer needs a dummy error code.
*   **Single-error `BadRequestException` constructors**: `new BadRequestException(error)` and `new BadRequestException(error, errorCode, message)` remove the need to wrap one `ValidationError` in an array.
*   **Entity/value convenience overloads**: `new NotFoundException(entity, value)` and `new ConflictException(entity, value)` (already available) are now complemented by `(entity, value, message)` overloads that keep `EntityName`/`EntityValue` populated while allowing a custom message.
*   **Optional messages everywhere**: every error-code overload accepts `string? message = null` and resolves it to `BaseResponseMessages.*` at runtime.

### 📝 Improvements

*   **Default messages resolved at runtime**: the message parameters are declared as `string? message = null` with a `?? BaseResponseMessages.*` fallback, so omitting the argument or passing `null` yields the package default. An explicit `""` is still preserved as an empty message.
*   **XML documentation corrected**: the `NotFoundException` summaries no longer say "generic conflict", and the `UnauthorizedException`/`ForbiddenException` single-message constructors are documented as custom-message constructors.
*   **Consistent constructor conventions**: all nine exceptions now follow the same default → error code + message → entity/value families, which keeps call sites stable when switching exception types.
*   **Documentation**: the Docusaurus package documentation now includes constructor tables, default-message tables, 401-vs-403 guidance, and the HTTP mapping for each type.

### 🔗 Dependency Update

*   **`RA.Utilities.Core.Constants` 10.0.5**: aligned with the `static readonly` constant fields introduced in Core.Constants v10.0.4, which is why the exception defaults are resolved with a nullable sentinel rather than a compile-time default value.

---

## Version 10.0.4

![Date Badge](https://img.shields.io/badge/Publish-27%20July%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.4-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.4)

This release modernizes the exception base class and aligns the entire exception hierarchy with the new `ResponseType` record from `RA.Utilities.Core.Constants` v10.0.2.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **`RaBaseException.ErrorCode` type changed from `string` to `int`**: The error code is now an integer, aligning with HTTP status code conventions. **Migration**: Replace string-based error codes (e.g., `nameof(BaseResponseCode.NotFound)`) with integer constants (e.g., `BaseResponseCode.NotFound`).
*   **`RaBaseException.ResponseCode` removed; replaced by `ResponseType`**: Use `ex.ErrorCode` for the HTTP status code and `ex.ResponseType` for the semantic type label. See the [Migration Guide](/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Exceptions/migration-guides) for detailed before/after examples.
*   **`RaBaseException` constructor signature changed**: Now accepts `(int errorCode, ResponseType errorType, string message)` instead of `(string errorCode, string message, int responseCode)`.
*   **`ValidationErrors` renamed to `ValidationError`**: The class now represents a single error (singular). A constructor argument `errorMessage` is now required. Properties are nullable (`string?` / `object?`).

### ✨ New Features

*   **`GatewayTimeoutException`**: A new exception class for HTTP 504 Gateway Timeout scenarios, such as when an upstream server fails to respond in time.
*   **Parameterless constructors**: `ForbiddenException`, `UnauthorizedException`, `UnprocessableException`, and `GatewayTimeoutException` now have parameterless constructors with sensible defaults.
*   **`string message` constructors**: These exception types now accept a single `string message` parameter for concise usage.
*   **`NotFoundException` and `ConflictException` generic constructors**: New constructors that accept only `errorCode` and `message` without requiring entity details.

### 📝 Improvements

*   **Properties use `init` accessors**: All exception properties (`EntityName`, `EntityValue`, `ErrorCode`, `ResponseType`, `Errors`) now use `{ get; init; }` for immutability after construction.
*   **`UnprocessableException` XML doc corrected**: Class summary now correctly documents HTTP 422 (was incorrectly referencing HTTP 409).
*   **`ValidationError` constructor**: Added a constructor accepting only `errorMessage` for simple error creation.
*   **XML Documentation**: All XML doc comments across the exception hierarchy have been reviewed and improved.

### 🔗 Dependency Update

*   **`RA.Utilities.Core.Constants`**: Updated to consume v10.0.2, which introduces the `ResponseType` record and `GatewayTimeout` constants.

---

## Version 10.0.3

![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.3)

This release enhances the `ConflictException` by adding a more flexible constructor.

<!-- truncate -->

### ✨ Improvements

*   **`ConflictException`**: Added a new constructor that allows throwing the exception with default conflict messages and codes, without requiring specific entity names or values. This is useful for general conflict scenarios where detailed entity information is not necessary or available.

## Version 10.0.2

![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.2)

This release introduces new semantic exceptions to handle authorization and state-based conflicts more effectively, increasing the granularity of error reporting within the RA.Utilities ecosystem.

<!-- truncate -->

### ✨ New Features

*   **`ForbiddenException`**: Introduced to represent scenarios where an authenticated user lacks sufficient permissions (maps to **HTTP 403 Forbidden**).
*   **`UnprocessableException`**: Added to represent operations that are invalid due to the current state of a resource (maps to **HTTP 422 Unprocessable Entity**).
*   **Documentation**: Updated the package README and added comprehensive documentation files with usage examples for the new exceptions.


## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.0)

Updated the project version from `10.0.100-rc.2` to the stable release version `10.0.0` in preparation for a production release.

Expanded documentation in the ***README*** with sections on best practices, how it works, and additional examples to improve user guidance and clarity on package usage. These enhancements aim to improve readability and usability of the documentation for users and developers.



### ✨ Key Documentation Improvements

*   **Added Table of Contents**: For easier navigation within the `README.md` file.
*   **New "How It Works" Section**: A new section was added to explain how the exceptions integrate with API middleware to standardize error handling.
*   **New "Best Practices" Section**: This new section provides clear guidelines on how to use the semantic exceptions correctly within a Clean Architecture, covering topics like where to throw exceptions and how to combine them with the `Result<T>` pattern.
*   **Improved Usage Examples**: The code examples for `NotFoundException`, `ConflictException`, and `BadRequestException` have been refined for better clarity.

### 📝 Notes

The goal of this update is to make the `RA.Utilities.Core.Exceptions` package more approachable and easier to adopt by providing comprehensive, easy-to-navigate documentation directly in the README.

## Version 10.0.100-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.100--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.100-rc.2)


This release focuses on clarifying the central role of semantic exceptions within the RA.Utilities ecosystem through greatly improved documentation and usage examples in related packages. While the exception classes themselves have not changed, their purpose and integration are now much clearer.

### ✨ Key Improvements

*   **Enhanced Documentation**:
    *   The `README.md` file has been updated to clearly articulate the purpose of using semantic exceptions like `NotFoundException` and `ConflictException`.
    *   New Docusaurus documentation provides a dedicated section for this package, making it easier for developers to find information.
*   **Clarified Integration with `Result<T>`**:
    *   Documentation for the `RA.Utilities.Core` package now explicitly demonstrates how these exceptions are used to represent the `Failure` state in the `Result` pattern. This highlights the primary mechanism for predictable error handling.
*   **Clarified Integration with the API Layer**:
    *   Documentation for `RA.Utilities.Api` (specifically `ErrorResultResponse` and `ErrorResultMapper`) now shows how these exceptions are automatically caught and translated into standardized HTTP error responses (e.g., 404, 409).

### 📝 Notes

The goal of this update is to improve the developer experience by making the error handling strategy of the RA.Utilities ecosystem transparent and easy to follow. By documenting how `RA.Utilities.Core.Exceptions` connects the business logic layer to the API layer, developers can more effectively build robust and predictable applications.
