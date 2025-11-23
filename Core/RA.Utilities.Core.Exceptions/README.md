# RA.Utilities.Core.Exceptions

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.Exceptions.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Core.Exceptions.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/)
[![Documentation](https://img.shields.io/badge/documentation-view-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/core/CoreExceptions/)

`RA.Utilities.Core.Exceptions` provides a set of standardized, semantic exceptions like `NotFoundException` and `ConflictException`. It solves the problem of using generic exceptions (e.g., `Exception` or `InvalidOperationException`) for predictable business rule failures.

By throwing exceptions that describe *what* went wrong (e.g., a resource was not found), you can create cleaner, more maintainable code. This allows other parts of your system, like API middleware, to catch specific exception types and produce standardized, meaningful error responses automatically.
- **Clear Intent**: Throwing a `NotFoundException` is more descriptive than a generic exception with a "not found" message.
- **Standardized Error Handling**: Middleware (like in `RA.Utilities.Api`) can catch these specific exception types and automatically map them to the correct HTTP status codes and structured error responses (e.g., 404 Not Found, 409 Conflict).
- **Decoupled Logic**: Your domain or application layer can focus on business rules and throw semantic exceptions without needing to know about HTTP details. The web layer handles the translation.
- **Reduced Boilerplate**: Eliminates the need for repetitive `try-catch` blocks in your controllers for common error scenarios.

---

## Table of Contents

- Getting started
- How It Works
- Available Exceptions
  - `NotFoundException`
  - `ConflictException`
  - `BadRequestException`
- Best Practices
- Additional documentation
- Contributing

---
## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core.Exceptions
```

Or through the NuGet Package Manager in Visual Studio.

---

## How It Works

This package is designed to integrate seamlessly with an API's error-handling middleware.

1.  **Business Logic**: Your service or application layer throws a semantic exception (e.g., `NotFoundException`) when a business rule is violated.
2.  **API Middleware**: In your API project (e.g., using `RA.Utilities.Api`), a global error-handling middleware catches these specific exceptions.
3.  **Automatic Mapping**: The middleware translates the exception into a standardized HTTP response (e.g., `NotFoundException` becomes `404 Not Found`).

This decouples your business logic from API concerns and standardizes error responses across your application.
---

## Available Exceptions

### `NotFoundException`

Inherits from `Exception`. Use this when a specific resource or entity cannot be found.

**Usage:**

```csharp
public Product GetProductById(int id)
{
    var product = _productRepository.Find(id);
    if (product == null)
    {
        throw new NotFoundException(nameof(product), id);
    }
    return product;
}
```

When caught by the `RA.Utilities.Api` middleware, this will typically be translated into an **HTTP 404 Not Found** response.

### `ConflictException`

Inherits from `Exception`. Use this when an action cannot be completed due to a conflict with the current state of a resource, such as trying to create a duplicate item.

**Usage:**

```csharp
public void CreateUser(string email)
{
    if (_userRepository.Exists(email))
    {
        throw new ConflictException("User", email);
    }
    // ... creation logic
}
```

This will typically be translated into an **HTTP 409 Conflict** response.

### `BadRequestException`

Inherits from `Exception`. Use this for client-side errors, such as invalid input or validation failures that are discovered in the business layer.

**Usage:**

```csharp
public void UpdateOrderStatus(int orderId, string newStatus)
{
    try
    {
        // Assume _validator is a FluentValidation validator
        await _validator.ValidateAndThrowAsync(command);
    }
    catch (ValidationException ex)
    {
        // Convert FluentValidation errors to our custom ValidationErrors
        var validationErrors = ex.Errors.Select(e => new ValidationErrors
        {
            PropertyName = e.PropertyName,
            ErrorMessage = e.ErrorMessage,
            AttemptedValue = e.AttemptedValue,
            ErrorCode = e.ErrorCode
        }).ToArray();

        // Return a failure Result containing the structured exception
        // highlight-next-line
        return new BadRequestException(validationErrors);
    }
    // ... update logic
}
```

This will typically be translated into an **HTTP 400 Bad Request** response.

---

## Best Practices

To get the most out of this exception model, follow these guidelines:

1.  **Throw from Business Logic, Catch in the API Layer**:
    Your services, domain, and application layers should be responsible for throwing these exceptions when a business rule is violated. The outermost layer of your application (e.g., the API project) should be responsible for catching them and translating them into the appropriate user-facing response. This keeps your core logic free of presentation concerns.

2.  **Be Specific**:
    Always use the most specific exception that fits the scenario. Throw a `NotFoundException` instead of a generic `Exception` with a "not found" message. This allows your error-handling middleware to act on the exception type, not the message text.

3.  **Use for Predictable Failures**:
    These exceptions are designed for *expected* and *predictable* business rule failures (e.g., a requested item doesn't exist, a user tries to register with a duplicate email). They are not intended for unexpected system errors like a database connection failure or a `NullReferenceException`.

4.  **Provide Clear, Log-Friendly Messages**:
    The exception message should clearly state what went wrong for logging and debugging purposes. However, avoid putting sensitive information in the message, as it might be logged or inadvertently exposed in an error response.

5.  **Combine with the `Result<T>` Pattern**:
    For a more functional approach, use the `Result<T>` object from `RA.Utilities.Core` to handle outcomes within your business logic. In your API or controller layer, you can then use the `Match` method to map the failure case directly to one of these exceptions, which the middleware will then handle automatically.

---

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](http://localhost:3000/RA.Utilities/nuget-packages/core/CoreExceptions/).

---

## Contributing

Contributions are welcome! If you have a suggestion for a new exception type or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.