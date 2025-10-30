---
title: RA.Utilities.Core.Constants
authors: [RedonAlla]
---

## Version 10.0.0-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.0-rc.2)


This release focuses on enhancing the clarity, consistency, and completeness of the constants provided by the package. The changes make the constants more intuitive to use and align the code with the documentation.

<!-- truncate -->

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