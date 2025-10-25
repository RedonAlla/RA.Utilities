```bash
Namespace: RA.Utilities.Core.Results
```

The `Result` type is a foundational class designed to handle operational outcomes in a clear, explicit, and robust manner.
It provides a functional approach to error handling, allowing you to represent both success and predictable failures without throwing exceptions.

## 🎯 Purpose

In many applications, exceptions are used for both unexpected system faults (like a database connection failure) and for expected, recoverable failures (like "User not found" or "Invalid input").
This can make error-handling code complex and difficult to read.

The `Result` type solves this by providing a wrapper that explicitly represents one of two outcomes:

- **Success**: The operation completed successfully and may contain a value.
- **Failure**: The operation failed and contains an `Exception` detailing the error.

This pattern encourages you to handle failures as part of your normal code flow, leading to more readable and resilient applications.

## The Result Types

- **`Result`**: Represents the outcome of an operation that does not return a value (like a `void` method).
It can be either a success or a failure.
- **`Result<T>`**: Represents the outcome of an operation that returns a value of type `T`.
It can be a success holding a `T` value, or a failure holding an `Exception`.

---

## `Result` Class

Used for `void`-like methods where you only need to know if the operation succeeded or failed.

### Properties

| Property | Type | Description |
|---|---|---|
| `IsSuccess` | `bool` | Returns `true` if the operation was successful. |
| `IsFailure` | `bool` | Returns `true` if the operation failed. |
| `Exception` | `Exception?` | Contains the exception if the operation failed; otherwise, it's `null`. |

### Example: Deleting a Resource

Instead of returning `void` or `bool`, a delete method can return a `Result`.

```csharp showLineNumbers
// Service Layer
public Result DeleteUser(int userId)
{
    var user = _userRepository.Find(userId);
    if (user == null)
    {
        // Return a failure result with a specific exception
        return new NotFoundException(nameof(User), userId);
    }

    _userRepository.Delete(user);

    // Return a success result
    return Result.Success();
}

// Consumer (e.g., an API Endpoint)
public IResult Delete(int id)
{
    Result result = _userService.DeleteUser(id);

    if (result.IsFailure)
    {
        // The ErrorResultResponse helper can map the exception to a proper HTTP response
        return ErrorResultResponse.Result(result.Exception);
    }

    return SuccessResponse.NoContent(); // HTTP 204
}
```

---

## `Result<TResult>` Class

A generic version of `Result` that holds a value of type `TResult` on success.

### Properties

| Property | Type | Description |
|---|---|---|
| `Value` | `TResult?` | Contains the result value if the operation was successful; otherwise, it's `default`. |
| `IsSuccess` | `bool` | (Inherited) Returns `true` if the operation was successful. |
| `IsFailure` | `bool` | (Inherited) Returns `true` if the operation failed. |
| `Exception` | `Exception?` | (Inherited) Contains the exception if the operation failed; otherwise, it's `null`. |

### `Match` Method

The `Match` method is the most powerful feature of the `Result<T>` type. It forces you to handle both the success and failure cases, eliminating the risk of unhandled errors. It takes two functions: one for success and one for failure.

`TContract Match<TContract>(Func<TResult, TContract> success, Func<Exception, TContract> failure)`

### Example: Fetching a Resource

```csharp showLineNumbers
// Service Layer
public Result<UserDto> GetUserById(int userId)
{
    var user = _userRepository.Find(userId);

    if (user == null)
    {
        // Failure: The user was not found
        return new NotFoundException(nameof(User), userId);
    }

    var userDto = new UserDto { Id = user.Id, Name = user.Name };
    
    // Success: Return the DTO wrapped in a Result
    return userDto; // Implicit conversion to Result.Success(userDto)
}

// Consumer (e.g., an API Endpoint) using Match
public IResult Get(int id)
{
    Result<UserDto> result = _userService.GetUserById(id);

    // The Match method elegantly handles both outcomes
    // highlight-start
    return result.Match<IResult>(
        success: userDto => SuccessResponse.Ok(userDto),
        failure: ErrorResultResponse.Result
    );
    // highlight-end
}
```

### Implicit Conversions

To make the syntax even cleaner, the `Result` types support implicit conversions from a value (for success) or an exception (for failure).

```csharp
public Result<string> GetGreeting(string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        // Implicitly convert an exception to a failure result
        return new BadRequestException("Name cannot be empty.");
    }

    // Implicitly convert a string to a success result
    return $"Hello, {name}!";
}
```

---

## Chaining Operations (Railway-Oriented Programming)

The `ResultExtensions` class provides a fluent API for chaining multiple operations that can fail. This pattern is often called "Railway-Oriented Programming," where the "happy path" (success track) continues, but any failure diverts the flow to the "failure track."

### `Map` - Transforming a Success Value

Use `Map` to transform the value inside a successful `Result<T>` without changing its success/failure state. If the result is a failure, the mapping function is skipped.

```csharp
// This method returns the user's email if found, or propagates the failure.
public Result<string> GetUserEmail(int userId)
{
    return _userRepository.FindUserById(userId) // Returns Result<User>
        .Map(user => user.Email); // Returns Result<string>
}
```

### `Bind` - Sequencing Operations

Use `Bind` to chain together multiple functions that each return a `Result`. The next function in the chain is only called if the previous one was successful.

```csharp
public Result<User> ValidateAndGetUser(int userId)
{
    return ValidateId(userId) // Returns Result
        .Bind(() => _userRepository.FindUserById(userId)); // Returns Result<User>
}
```

### `OnSuccess` / `OnFailure` - Performing Side-Effects

These methods are for executing actions that don't affect the result, such as logging.

```csharp
public Result<User> FindAndLogUser(int userId)
{
    return _userRepository.FindUserById(userId)
        .OnSuccess(user => _logger.LogInformation("Successfully retrieved user {Name}", user.Name))
        .OnFailure(ex => _logger.LogError(ex, "Failed to find user"));
}
```

### Asynchronous Operations

The library provides `async` versions of the extension methods (`MapAsync`, `BindAsync`, etc.) to seamlessly integrate with asynchronous workflows.

```csharp
public async Task<Result<string>> GetUserNameAsync(int userId)
{
    // Assume FindByIdAsync returns Task<Result<User>>
    Task<Result<User>> userResultTask = _userRepository.FindByIdAsync(userId);
    // highlight-next-line
    return await userResultTask.MapAsync(user => user.Name);
}
```

