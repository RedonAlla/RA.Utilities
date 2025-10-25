---
title: ConflictException
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `ConflictException` is a semantic exception used to indicate that a request could not be processed because of a conflict with the current state of the target resource. It is designed to be translated into an **HTTP 409 Conflict** response.

## 🎯 Purpose

This exception is used for predictable business-level failures where an operation violates a unique constraint or a concurrency check. The most common scenario is attempting to create a resource that already exists (e.g., a user with a duplicate email address).

When used with the `Result` pattern, a `ConflictException` is returned inside a `Failure` result. The API layer can then inspect this exception and automatically generate a standardized 409 response, providing clear feedback to the client about the nature of the conflict.

## Properties

| Property      | Type     | Description                                                          |
|---------------|----------|----------------------------------------------------------------------|
| **EntityName**  | `string` | The name of the entity type that caused the conflict (e.g., "User"). |
| **EntityValue** | `object` | The value or identifier of the conflicting entity (e.g., the email address). |

## 🚀 How to Use

You will typically create and return a `ConflictException` from a service or handler when a business rule for uniqueness is violated.

### Example: Creating a Duplicate Resource

In this example, a service attempts to create a new user but finds that a user with the same email already exists.

```csharp showLineNumbers
// In a CQRS handler or application service
using RA.Utilities.Core;
// highlight-next-line
using RA.Utilities.Core.Exceptions;

public async Task<Result<UserDto>> CreateUserAsync(CreateUserCommand command)
{
    // Check if a user with this email already exists in the database
    bool userExists = await _userRepository.ExistsAsync(command.Email);

    if (userExists)
    {
        // Return a failure Result containing a ConflictException
        // highlight-next-line
        return new ConflictException(nameof(User), command.Email);
    }

    // ... continue with user creation logic
}
```

### Example JSON Output

When the API layer (using `ErrorResultResponse`) handles the `Failure` `Result` from the example above, it will automatically generate a `409 Conflict` response with a body like this:

```json showLineNumbers
{
  "responseCode": 409,
  "responseType": "Conflict",
  "responseMessage": "User with value 'test@example.com' already exists.",
  "result": {
    "entityName": "User",
    "entityValue": "test@example.com"
  }
}
```
