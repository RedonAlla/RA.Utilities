# RA.Utilities.Core

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Core.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/core/UtilitiesCore/)

`RA.Utilities.Core` is a lightweight .NET library designed to enhance error handling by providing a functional `Result` type. It helps you write cleaner, more predictable, and more robust code by avoiding exceptions for expected operational failures.

## 🚀 Getting Started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core
```

## ✨ Key Features

### Functional `Result` and `Result<T>` Types

The library provides two core types for handling outcomes:
*   `Result`: For operations that succeed or fail without returning a value (e.g., saving to a database).
*   `Result<T>`: For operations that return a value on success.

These types make the intent of your code explicit by forcing you to handle both success and failure paths, preventing unhandled errors.

```csharp
public Result<User> FindUser(int userId)
{
    var user = _userRepository.GetById(userId);

    if (user is null)
    {
        return new NotFoundException("User not found."); // Implicit conversion to Failure
    }

    return user; // Implicit conversion to Success
}
```

The `Match` method ensures that both success and failure cases are handled:

```csharp
var result = FindUser(123);

return result.Match(
    onSuccess: user => Ok(user),
    onFailure: exception => NotFound(exception.Message)
);
```

### Railway-Oriented Programming Extensions

Chain multiple fallible operations together using a fluent, readable API. The chain short-circuits on the first failure, passing the error down the line.

*   **`Map()`**: Transforms the value inside a successful `Result<T>`.
*   **`Bind()`**: Chains another function that returns a `Result`.
*   **`OnSuccess()` / `OnFailure()`**: Executes side-effects like logging without altering the result.

```csharp
public Result<string> GetAdminEmail(int userId)
{
    return FindUser(userId)
        .Bind(user => EnsureIsAdmin(user))
        .Map(adminUser => adminUser.Email)
        .OnSuccess(email => _logger.LogInformation("Admin email found for user {UserId}", userId))
        .OnFailure(ex => _logger.LogError(ex, "Failed to get admin email for user {UserId}", userId));
}
```

### Asynchronous Support

All extension methods have `async` counterparts (`MapAsync`, `BindAsync`, etc.) to integrate seamlessly into asynchronous workflows.

```csharp
var result = await FindUserAsync(userId)
    .BindAsync(user => ValidatePermissionsAsync(user));
```


## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/core/UtilitiesCore/).