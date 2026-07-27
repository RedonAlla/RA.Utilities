# Release Notes for RA.Utilities.Core.Exceptions

## Version 10.0.4

![Date Badge](https://img.shields.io/badge/Publish-27%20July%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.4-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.4)

This release modernizes the exception base class and aligns the entire exception hierarchy with the new `ResponseType` record from `RA.Utilities.Core.Constants` v10.0.2. The changes improve type safety, extensibility, and developer ergonomics.

### ⚠️ Breaking Changes

*   **`RaBaseException.ErrorCode` type changed from `string` to `int`**: The error code is now an integer, aligning with HTTP status code conventions and the constants defined in `BaseResponseCode`. **Migration**: Replace any string-based error code assignments (e.g., `nameof(BaseResponseCode.NotFound)`) with the corresponding integer constant (e.g., `BaseResponseCode.NotFound`).
*   **`RaBaseException.ResponseCode` removed; replaced by `ResponseType`**: The `ResponseCode` property (an `int`) has been removed and replaced with `ResponseType ResponseType` (the new `record` type from `RA.Utilities.Core.Constants`). This provides richer semantic meaning for error categorization. **Migration**: Replace references to `ex.ResponseCode` with `ex.ErrorCode` for the HTTP status code, and use `ex.ResponseType` for the semantic type label.
*   **`RaBaseException` constructor signature changed**: The constructor now accepts `(int errorCode, ResponseType errorType, string message)` instead of the previous `(string errorCode, string message, int responseCode)`. All derived exception constructors have been updated accordingly.
*   **`ValidationErrors` renamed to `ValidationError`**: The class representing a single validation error has been renamed from `ValidationErrors` (plural) to `ValidationError` (singular) to better reflect that it represents a single error. **Migration**: Rename all usages of `ValidationErrors` to `ValidationError`.
*   **`ValidationError` properties are now nullable**: `PropertyName`, `ErrorMessage`, `ErrorCode`, `AttemptedValue`, and `ExpectedValue` are now nullable (`string?` / `object?`). A new constructor `ValidationError(string errorMessage)` has been added for simple error creation.

### ✨ New Features

*   **`TooManyRequestsException`**: A new exception class for HTTP 429 Too Many Requests scenarios, such as rate limiting, throttling, and quota enforcement.
*   **`ServiceUnavailableException`**: A new exception class for HTTP 503 Service Unavailable scenarios, such as maintenance mode, circuit breaker trips, and dependency outages.
*   **`GatewayTimeoutException`**: A new exception class for HTTP 504 Gateway Timeout scenarios, such as when an upstream server fails to respond in time. Provides parameterless, `string message`, and `(int errorCode, string message)` constructors, consistent with the other exception types.
*   **Parameterless constructors added to all exception types**: `ForbiddenException`, `UnauthorizedException`, `UnprocessableException`, `GatewayTimeoutException`, `TooManyRequestsException`, and `ServiceUnavailableException` now have parameterless constructors that use sensible defaults from `BaseResponseCode` and `BaseResponseMessages`. `NotFoundException` and `ConflictException` gained generic constructors that accept only `errorCode` and `message` without requiring entity details.
*   **`string message` constructors added**: `ForbiddenException`, `UnauthorizedException`, `UnprocessableException`, `GatewayTimeoutException`, `TooManyRequestsException`, and `ServiceUnavailableException` now have single-parameter constructors accepting a custom message while using default error codes and response types.

### 📝 Improvements

*   **Properties use `init` accessors**: All exception properties (`EntityName`, `EntityValue`, `ErrorCode`, `ResponseType`, `Errors`) now use `{ get; init; }` instead of `{ get; }` or `{ get; set; }`, making exceptions immutable after construction.
*   **`UnprocessableException` XML doc corrected**: The class summary previously referenced HTTP 409 (Conflict) but now correctly documents HTTP 422 (Unprocessable Entity).
*   **XML Documentation**: All XML doc comments across the exception hierarchy have been reviewed and improved for clarity.
*   **`ValidationError` constructor**: Added a constructor accepting only `errorMessage` for the common case where only a message is needed.

### 🔗 Dependency Update

*   **`RA.Utilities.Core.Constants`**: Updated to consume v10.0.2, which introduces the `ResponseType` record and `GatewayTimeout` constants.

---

## Version 10.0.3

![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.3)

This release enhances the `ConflictException` by adding a more flexible constructor.

### ✨ Improvements

*   **`ConflictException`**: Added a new constructor that allows throwing the exception with default conflict messages and codes, without requiring specific entity names or values. This is useful for general conflict scenarios where detailed entity information is not necessary or available.


## Version 10.0.2

![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.2)

This release introduces new semantic exceptions to handle authorization and state-based conflicts more effectively, increasing the granularity of error reporting within the RA.Utilities ecosystem.

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

---

## Version 10.0.100-rc.2
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
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

---

Thank you for using RA.Utilities!
