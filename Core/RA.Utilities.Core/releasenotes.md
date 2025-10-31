[![NuGet version](https://img.shields.io/badge/NuGet-10.0.100--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core/10.0.100-rc.2)

This is the initial release of `RA.Utilities.Core`, a lightweight .NET library designed to enhance error handling by providing a functional `Result` type. It helps you write cleaner, more predictable, and more robust code by avoiding exceptions for expected operational failures.

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
dotnet add package RA.Utilities.Core
```