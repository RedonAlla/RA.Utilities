<p align="center">
  <img src="../../Assets/Images/core.svg" alt="RA.Utilities.Core Logo" width="128">
</p>

# RA.Utilities.Core

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities/)
[![codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg?token=10WESSLKL6)](https://codecov.io/github/RedonAlla/RA.Utilities)

`RA.Utilities.Core` is a lightweight .NET library designed to enhance error handling by providing a functional `Result` type.
It helps you write cleaner, more predictable, and more robust code by avoiding exceptions for expected operational failures.
This approach allows for a clear and explicit representation of success or failure, making your application's control flow easier to follow and maintain.

## Purpose

In many applications, exceptions are used for both unexpected system faults (e.g., `NullReferenceException`) and predictable business-level failures (e.g., "user not found").
This can lead to complex `try-catch` blocks and makes it difficult to distinguish between recoverable errors and true exceptions.

The `Result` type solves this by providing a wrapper that explicitly represents one of two outcomes:

- **Success**: The operation completed successfully, and may contain a value.
- **Failure**: The operation failed, and contains an `Exception` detailing the error.

This pattern encourages you to handle failures as part of your normal code flow, leading to more readable and resilient applications.

## 🛠️ Installation

You can install the package via the .NET CLI:

```sh
dotnet add package RA.Utilities.Core
```

Or through the NuGet Package Manager in Visual Studio.

---

## When to use Results Pattern and when to use Exceptions 

| Layer	               | Use Results Pattern | Use Exceptions |
| -------------------- | ------------------- | -------------- |
| ✅ Application Layer | ✅ Yes              | ❌ Avoid       |
| ✅ Domain Layer      | ✅ Yes              | ❌ Avoid       |
| 🌐 Infrastructure    | ❌ Rarely           | ✅ Yes         |
| 🌍 External APIs     | ❌                  | ✅ Required    |
| 🧪 Tests             | ✅ Preferred        | ✅ Avoid       |

| Situation	                    | Use               |
| ----------------------------- | ----------------- |
| Input validation failed       | ✅ Result pattern |
| Business rule not satisfied   | ✅ Result pattern |
| DB connection failed          | ⚠️ Exception      |
| File not found (unexpected)   | ⚠️ Exception      |
| You want to chain logic steps | ✅ Result pattern |
| You need a stack trace.       | ⚠️ Exception      |

## The Result Type

- `Result`: Represents the outcome of an operation that does not return a value. It can be either a success or a failure.
- `Result<T>`: Represents the outcome of an operation that returns a value of type `T`. It can be a success holding a `T` value, or a failure holding an `Exception`.

### `Result` Class

Used for void-like methods where you only need to know if the operation succeeded or failed.

#### Properties

| Property    | Type         | Description                                                              |
|-------------|--------------|--------------------------------------------------------------------------|
| `IsSuccess` | `bool`       | Returns `true` if the operation was successful.                          |
| `IsFailure` | `bool`       | Returns `true` if the operation failed.                                  |
| `Exception` | `Exception?` | Contains the exception if the operation failed; otherwise, it's `null`. |

#### Factory Methods

- `Result.Success()`: Creates a success result.
- `Result.Failure(Exception exception)`: Creates a failure result.

#### Example: Deleting a Resource

Imagine a service method that deletes a user. Instead of returning `void` or `bool`, it can return a `Result`.

```csharp
// Service Layer
public Result DeleteUser(int userId)
{
    try
    {
        var user = _userRepository.Find(userId);
        if (user == null)
        {
            // Return a failure result with a specific exception
            return Result.Failure(new KeyNotFoundException($"User with ID {userId} not found."));
        }

        _userRepository.Delete(user);

        // Return a success result
        return Result.Success();
    }
    catch (Exception ex)
    {
        // Wrap unexpected exceptions in a failure result
        return Result.Failure(ex);
    }
}

// Consumer (e.g., an API Controller)
public IActionResult Delete(int id)
{
    Result result = _userService.DeleteUser(id);

    if (result.IsFailure)
    {
        // Handle specific, known failures
        if (result.Exception is KeyNotFoundException)
        {
            return NotFound(result.Exception.Message);
        }
        // Handle other unexpected failures
        return StatusCode(500, "An internal error occurred.");
    }

    return NoContent(); // HTTP 204
}
```

### `Result<TResult>` Class

A generic version of `Result` that holds a value of type `TResult` on success.

#### Properties

| Property    | Type         | Description                                                                             |
|-------------|--------------|-----------------------------------------------------------------------------------------|
| `Value`     | `TResult?`   | Contains the result value if the operation was successful; otherwise, it's `default`.   |
| `IsSuccess` | `bool`       | (Inherited) Returns `true` if the operation was successful.                             |
| `IsFailure` | `bool`       | (Inherited) Returns `true` if the operation failed.                                     |
| `Exception` | `Exception?` | (Inherited) Contains the exception if the operation failed; otherwise, it's `null`.    |


#### Factory Methods

- `Result.Success<TResult>(TResult value)`: Creates a success result with a value.
- `Result.Failure<TResult>(Exception exception)`: Creates a failure result for a generic type.

#### `Match` Method

The `Match` method is the most powerful feature of the `Result<T>` type.
It forces you to handle both the success and failure cases, eliminating the risk of unhandled errors.
It takes two functions: one for success and one for failure.

`TContract Match<TContract>(Func<TResult, TContract> success, Func<Exception, TContract> failure)`

```csharp
var userRepository = new UserRepository();
Result<User> result = userRepository.FindUserById(1);

string message = result.Match(
    success: user => $"User found: {user.Name}",
    failure: ex => $"Error: {ex.Message}"
);

Console.WriteLine(message);
```

#### Example: Fetching a Resource

Here, a service method fetches a `UserDto`.

```csharp
// Service Layer
public Result<UserDto> GetUserById(int userId)
{
    var user = _userRepository.Find(userId);

    if (user == null)
    {
        // Failure: The user was not found
        return Result.Failure<UserDto>(new KeyNotFoundException($"User with ID {userId} not found."));
    }

    var userDto = new UserDto { Id = user.Id, Name = user.Name };
    
    // Success: Return the DTO wrapped in a Result
    return Result.Success(userDto);
}

// Consumer (e.g., an API Controller) using Match
public IActionResult Get(int id)
{
    Result<UserDto> result = _userService.GetUserById(id);

    // The Match method elegantly handles both outcomes
    return result.Match<IActionResult>(
        success: userDto => Ok(userDto),
        failure: ex =>
        {
            if (ex is KeyNotFoundException)
                return NotFound(ex.Message);
            
            // Log the exception for diagnostics
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500, "An internal error occurred.");
        }
    );
}
```

### Implicit Conversions

To make the syntax even cleaner, the `Result` types support implicit conversions.

- A `TResult` can be implicitly converted to `Result<TResult>` (success).
- An `Exception` can be implicitly converted to `Result` or `Result<TResult>` (failure).

```csharp
public Result<string> GetGreeting(string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        // Implicitly convert an exception to a failure result
        return new ArgumentException("Name cannot be empty.", nameof(name));
    }

    // Implicitly convert a string to a success result
    return $"Hello, {name}!";
}
```

### Chaining Operations (Railway-Oriented Programming)

The `ResultExtensions` class provides a fluent API for chaining multiple operations that can fail. This pattern is often called "Railway-Oriented Programming," where the "happy path" (success track) continues, but any failure diverts the flow to the "failure track."

#### `Map` - Transforming a Success Value

Use `Map` to transform the value inside a successful `Result<T>` without changing the result's success/failure state. If the result is a failure, the mapping function is skipped.

```csharp
// This method returns the user's email if found, or propagates the failure.
public Result<string> GetUserEmail(int userId)
{
    var userRepository = new UserRepository();
    return userRepository.FindUserById(userId)
        .Map(user => user.Email);
}
```

#### `Bind` - Sequencing Operations

Use `Bind` to chain together multiple functions that each return a `Result`. The next function in the chain is only called if the previous one was successful.

```csharp
public Result ValidateAndGetUser(int userId)
{
    return ValidateId(userId) // Assuming this returns a Result
        .Bind(() => new UserRepository().FindUserById(userId)); // Returns Result<User>, which is compatible
}
```

#### `OnSuccess` / `OnFailure` - Performing Side-Effects

These methods are for executing actions that don't affect the result, such as logging.

```csharp
public Result<User> FindAndLogUser(int userId)
{
    var userRepository = new UserRepository();
    return userRepository.FindUserById(userId)
        .OnSuccess(user => Console.WriteLine($"Successfully retrieved user {user.Name}"))
        .OnFailure(ex => Console.WriteLine($"Failed to find user: {ex.Message}"));
}
```

### Asynchronous Operations

The library provides `async` versions of the extension methods to seamlessly integrate with asynchronous workflows. All async extensions end with the `Async` suffix (e.g., `MapAsync`, `BindAsync`).

```csharp
public async Task<Result<string>> GetUserNameAsync(int userId)
{
    // Assume FindByIdAsync returns Task<Result<User>>
    Task<Result<User>> userResultTask = _userRepository.FindByIdAsync(userId);

    return await userResultTask.MapAsync(user => user.Name);
}

public async Task ProcessUserAsync(int userId)
{
    await GetUserNameAsync(userId)
        .OnSuccessAsync(async name => await _emailService.SendWelcomeEmailAsync(name))
        .OnFailureAsync(async ex => await _logger.LogAsync(ex));
}
```

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-result-extensions` or `fix/readme-typo`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
```