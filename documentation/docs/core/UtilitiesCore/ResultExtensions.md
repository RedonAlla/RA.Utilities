```bash
Namespace: RA.Utilities.Core.Results
```

The `ResultExtensions` class provides a set of powerful extension methods for the
[`Result`](./Results.md#result-class) and [`Result<T>`](./Results.md#resulttresult-class) types.
These methods enable a fluent, functional style for chaining operations, often called **Railway-Oriented Programming**.

## 🎯 Purpose

The `ResultExtensions` class is a static class that adds a powerful set of extension methods to the [`Result`](./Results.md#result-class) and [`Result<T>`](./Results.md#resulttresult-class) types.
Its primary purpose is to enable a fluent, functional programming style often referred to as **Railway-Oriented Programming**.

Imagine your code has two parallel tracks: a "success track" and a "failure track."

* When an operation is successful, it stays on the success track, and you can continue to chain more operations.
* As soon as an operation fails, the [`Result`](./Results.md) is switched to the failure track. All subsequent operations in the chain are automatically skipped, and the original failure is carried through to the end.

This pattern allows you to write clean, linear, and highly readable code for complex workflows without nested `if` statements or `try-catch` blocks for predictable failures.
You can compose a sequence of steps, and the extensions will handle the flow control for you.

The key methods that enable this are:

* **`Map`**: Transforms the value inside a successful [`Result`](./Results.md) into a new value.
* **`Bind`**: Chains together multiple operations that each return a [`Result`](./Results.md).
* **`OnSuccess`/ `OnFailure`** : Executes side-effects (like logging) without changing the [`Result`](./Results.md).
* **`Match`**: Provides a final, explicit way to handle both the success and failure outcomes.

The class also provides `async` versions of these methods (`MapAsync`, `BindAsync`, etc.) to work seamlessly with asynchronous code.

---

## Chaining Methods

### `Map` - Transforming a Success Value

Use `Map` when you want to transform the value inside a successful [`Result<T>`](./Results.md#resulttresult-class) into a new value.
The operation should be one that cannot fail. If the [`Result`](./Results.md) is already a failure,
the mapping function is ignored, and the failure is passed through.

`Map` is the equivalent of `Select` in LINQ.

#### Example

```csharp showLineNumbers
public Result<string> GetUserName(int userId)
{
    // Assume FindById returns Result<User>
    Result<User> userResult = _userRepository.FindById(userId);

    // If userResult is a success, map the User object to its Name property.
    // If it's a failure, the failure is propagated to userNameResult.
    // highlight-next-line
    Result<string> userNameResult = userResult.Map(user => user.Name);

    return userNameResult;
}
```

### `Bind` - Sequencing Operations That Can Fail

Use `Bind` (also known as `SelectMany` or `FlatMap`) to chain together multiple functions that **each return a `Result`**. This is the cornerstone of railway-oriented programming. The next function in the chain is only executed if the previous one was successful.

#### Example

Imagine a workflow:
1.  Validate an ID (`Result`).
2.  Find a user by that ID (`Result<User>`).
3.  Check if the user has an active subscription (`Result<User>`).

```csharp showLineNumbers
public Result<User> GetSubscribedUser(int userId)
{
    return ValidateId(userId) // Returns Result
        .Bind(() => _userRepository.FindById(userId)) // Returns Result<User>
        .Bind(user => CheckSubscription(user)); // Returns Result<User>
}

// Helper methods for the example
private Result ValidateId(int id)
{
    return id > 0 ? Result.Success() : new BadRequestException("Invalid ID");
}

private Result<User> CheckSubscription(User user)
{
    return user.IsSubscribed ? user : new BadRequestException("User is not subscribed.");
}
```

In the example above, if `ValidateId` fails, neither `FindById` nor `CheckSubscription` will be executed.
The `BadRequestException` from `ValidateId` will be returned immediately.

### `OnSuccess` & `OnFailure` - Performing Side-Effects

Use `OnSuccess` or `OnFailure` to perform an action that doesn't change the outcome of the `Result`. These are perfect for side-effects like logging. The original `Result` is always returned, allowing you to continue the chain.

#### Example

```csharp showLineNumbers
public Result<User> FindAndLogUser(int userId)
{
    return _userRepository.FindById(userId)
        .OnSuccess(user => _logger.LogInformation("User {Name} found.", user.Name))
        .OnFailure(ex => _logger.LogError(ex, "Failed to find user with ID {UserId}.", userId));
}
```

---

## Terminal Methods

### `Match` - Handling Both Outcomes

The `Match` extension method is a terminal operation that forces you to explicitly handle both the success and failure cases. It's the primary way to "exit" the `Result` chain and produce a final value (like an `IResult` in an API).

#### Example

```csharp showLineNumbers
// In a Minimal API endpoint
app.MapGet("/users/{id}", (int id) =>
{
    // highlight-start
    return GetSubscribedUser(id) // This returns a Result<User>
        .Match<IResult>(
            success: user => SuccessResponse.Ok(user),
            failure: ErrorResultResponse.Result
        );
    // highlight-end
});
```

---

## Asynchronous Operations

The `ResultExtensions` class provides `async` versions for all the key chaining methods to support asynchronous workflows. These methods work on `Task<Result>` and `Task<Result<T>>`.

- `MapAsync`
- `BindAsync`
- `OnSuccessAsync`
- `OnFailureAsync`

### Example: Async `Bind`

```csharp showLineNumbers
public async Task<Result<string>> GetUserEmailAsync(int userId)
{
    // Assume FindByIdAsync returns Task<Result<User>>
    Task<Result<User>> userResultTask = _userRepository.FindByIdAsync(userId);

    // Use await and MapAsync to transform the result when it's ready.
    return await userResultTask.MapAsync(user => user.Email);
}

public async Task<Result> ProcessUserAsync(int userId)
{
    // Chain multiple async operations together
    return await ValidateIdAsync(userId) // Returns Task<Result>
        .BindAsync(() => FindUserAsync(userId)) // Returns Task<Result<User>>
        .BindAsync(user => SendWelcomeEmailAsync(user)); // Returns Task<Result>
}
```

By using these extensions, you can build robust, resilient, and highly readable application logic that clearly separates success paths from failure paths.
