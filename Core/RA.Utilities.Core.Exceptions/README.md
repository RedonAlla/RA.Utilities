# RA.Utilities.Core.Exceptions

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.Exceptions.svg)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Core.Exceptions.svg)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/)

`RA.Utilities.Core.Exceptions` provides a set of standardized, semantic exceptions like `NotFoundException` and `ConflictException`. It solves the problem of using generic exceptions (e.g., `Exception` or `InvalidOperationException`) for predictable business rule failures.

By throwing exceptions that describe *what* went wrong (e.g., a resource was not found), you can create cleaner, more maintainable code. This allows other parts of your system, like API middleware, to catch specific exception types and produce standardized, meaningful error responses automatically.
- **Clear Intent**: Throwing a `NotFoundException` is more descriptive than a generic exception with a "not found" message.
- **Standardized Error Handling**: Middleware (like in `RA.Utilities.Api`) can catch these specific exception types and automatically map them to the correct HTTP status codes and structured error responses (e.g., 404 Not Found, 409 Conflict).
- **Decoupled Logic**: Your domain or application layer can focus on business rules and throw semantic exceptions without needing to know about HTTP details. The web layer handles the translation.
- **Reduced Boilerplate**: Eliminates the need for repetitive `try-catch` blocks in your controllers for common error scenarios.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core.Exceptions
```

Or through the NuGet Package Manager in Visual Studio.

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
        throw new NotFoundException("Product", id);
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
    if (string.IsNullOrWhiteSpace(newStatus))
    {
        throw new BadRequestException("Status", "Order status cannot be empty.");
    }
    // ... update logic
}
```

This will typically be translated into an **HTTP 400 Bad Request** response.

---

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](http://localhost:3000/RA.Utilities/nuget-packages/core/CoreExceptions/).

---

## Contributing

Contributions are welcome! If you have a suggestion for a new exception type or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.