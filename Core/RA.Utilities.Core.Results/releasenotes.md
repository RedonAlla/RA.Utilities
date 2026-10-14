## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-14%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Results/10.0.1)

The `Result` extension methods are reorganized into four classes under the new `RA.Utilities.Core.Results.Extensions` namespace, and every method is now declared as a C# 14 extension member.

### ⚠️ Breaking Changes

*   **Extensions moved to the `RA.Utilities.Core.Results.Extensions` namespace**: `Result` and `Result<T>` stay in `RA.Utilities.Core.Results`, but `Map`, `Bind`, `OnSuccess`, `OnFailure`, `Match` and their async counterparts now require `using RA.Utilities.Core.Results.Extensions;`. Without it, call sites no longer compile (`CS1061`). See the [migration guide](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Results/migration-guides).
*   **`ResultExtensions` split into four classes**: `ResultExtensions` (on `Result`), `TypedResultExtensions` (on `Result<T>`), `ResultAsyncExtensions` (on `Task<Result>`) and `TypedResultAsyncExtensions` (on `Task<Result<T>>`). Method names, signatures, and call sites are unchanged — only code that references the class by name breaks.

### 📝 Improvements

*   All extension methods are declared with C# 14 extension members inside `extension(...)` blocks on their target type; the generated methods have identical names and signatures.

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-19%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Results/10.0.0)

The project version has been updated from `10.0.100-rc.2` to the stable release version `10.0.0` in preparation for a production release.

## Version 10.0.100-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.100--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core/10.0.100-rc.2)

This is the initial release of `RA.Utilities.Core.Results`, a lightweight .NET library designed to enhance error handling by providing a functional `Result` type. It helps you write cleaner, more predictable, and more robust code by avoiding exceptions for expected operational failures.


### ✨ Key Features

*   **Functional `Result` and `Result<T>` Types**:
    *   Provides `Result` for operations that don't return a value and `Result<T>` for those that do.
    *   Clearly separates success and failure paths, making your code's intent explicit.
    *   The `Match` method ensures that both success and failure cases are handled, preventing unhandled errors.

*   **Railway-Oriented Programming Extensions**:
    *   Includes a fluent API for chaining multiple operations that can fail.
    *   `Map()`: Transforms the value inside a successful `Result<T>`.
    *   `Bind()`: Chains multiple functions that each return a `Result`.
    *   `OnSuccess()` / `OnFailure()`: Executes side-effects like logging without altering the result.

*   **Asynchronous Support**:
    *   Provides `async` versions of all extension methods (`MapAsync`, `BindAsync`, etc.) to seamlessly integrate with asynchronous workflows.

*   **Implicit Conversions**:
    *   Simplifies your code by allowing implicit conversions from a value to a success `Result<T>` and from an `Exception` to a failure `Result`.

### 🚀 Getting Started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core.Results
```