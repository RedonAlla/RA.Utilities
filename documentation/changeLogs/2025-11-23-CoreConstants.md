---
title: RA.Utilities.Core.Constants
authors: [RedonAlla]
---

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