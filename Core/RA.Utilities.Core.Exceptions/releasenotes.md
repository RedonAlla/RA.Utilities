# Release Notes for RA.Utilities.Core.Exceptions

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
