# Release Notes for RA.Utilities.Core.Exceptions

## Version 10.0.100-rc.2

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
