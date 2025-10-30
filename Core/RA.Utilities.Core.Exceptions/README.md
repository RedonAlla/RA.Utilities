<p align="center">
  <img src="../../Assets/Images/exceptions.png" alt="RA.Utilities.Core.Exceptions Logo" width="128">
</p>

# RA.Utilities.Core.Exceptions

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.Exceptions?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/)

`RA.Utilities.Core.Exceptions` provides a set of standardized, semantic exceptions for use across the RA.Utilities ecosystem. These exceptions, such as `NotFoundException` and `ConflictException`, allow for clear, intent-driven error handling in your business logic.

## Purpose

Instead of throwing generic `Exception` or `ArgumentException` types, this package provides exceptions that carry semantic meaning about what went wrong. This approach has several key benefits:

- **Clear Intent**: Throwing a `NotFoundException` is more descriptive than a generic exception with a "not found" message.
- **Standardized Error Handling**: Middleware (like in `RA.Utilities.Api`) can catch these specific exception types and automatically map them to the correct HTTP status codes and structured error responses (e.g., 404 Not Found, 409 Conflict).
- **Decoupled Logic**: Your domain or application layer can focus on business rules and throw semantic exceptions without needing to know about HTTP details. The web layer handles the translation.
- **Reduced Boilerplate**: Eliminates the need for repetitive `try-catch` blocks in your controllers for common error scenarios.

## 🛠️ Installation

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

## Contributing

Contributions are welcome! If you have a suggestion for a new exception type or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.