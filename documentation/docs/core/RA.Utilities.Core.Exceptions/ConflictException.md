---
title: ConflictException
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`ConflictException` signals that a request cannot be processed because it conflicts with the current state of the target resource. The API layer maps it to **HTTP 409 Conflict**.

## 🎯 Purpose

Use this exception for predictable failures that violate a uniqueness constraint or a concurrency check. The most common case is creating a resource that already exists (for example, a user with a duplicate email address).

Common scenarios include:

- Creating an entity whose unique key already exists.
- Deleting a user who still has active orders (state conflict).
- Optimistic concurrency failures (version mismatch).

The client may be able to resolve a conflict by inspecting the current state and retrying with different data.

## Properties

| Property | Type | Description |
|---|---|---|
| **`EntityName`** | `string` | Name of the entity that caused the conflict (e.g., `"User"`). Set only by the entity/value constructors. |
| **`EntityValue`** | `object` | Value or identifier of the conflicting entity (e.g., the email address). Set only by the entity/value constructors. |
| `ErrorCode` | `int` | Inherited. `409` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.Conflict`. |
| `Message` | `string` | Inherited. The supplied message when one is given, the entity template for entity/value constructors, otherwise `BaseResponseMessages.Conflict`. |

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `ConflictException()` | `409` | `BaseResponseMessages.Conflict` |
| `ConflictException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.Conflict` when `null`/omitted |
| `ConflictException(string entity, object value)` | `409` | `"{entity} with value '{value}' already exists."` |
| `ConflictException(string entity, object value, string? message)` | `409` | supplied, or the entity template when `null` |
| `ConflictException(string entity, object value, int errorCode, string? message = null)` | supplied | supplied, or the entity template when `null`/omitted |

The entity/value constructors set `EntityName` and `EntityValue`; the others leave both unset.

## 🚀 How to Use

### Example: Creating a duplicate resource

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result<UserDto>> CreateUserAsync(CreateUserCommand command)
{
    bool userExists = await _userRepository.ExistsAsync(command.Email);

    if (userExists)
    {
        // highlight-next-line
        return new ConflictException(nameof(User), command.Email);
    }

    // ... continue with user creation logic
}
```

### Example: A generic conflict

```csharp showLineNumbers
// 409 / ResponseType.Conflict / BaseResponseMessages.Conflict
return new ConflictException();

// 409 / custom code / default message
return new ConflictException(errorCode: 4090);
```

### Example: Custom message with entity details

Passing a message keeps `EntityName` and `EntityValue` populated:

```csharp showLineNumbers
return new ConflictException(
    entity: nameof(User),
    value: command.Email,
    message: "An account with this email is already active."
);
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `ConflictException` to **HTTP 409** with a `ConflictResponse` body containing `entity` and `value` from the exception. The `ErrorCode` is echoed into `responseCode`; it does not change the HTTP status.

See [`ErrorResultResponse`](../../api/RA.Utilities.Api/mapper/ErrorResultResponse.md) for the response shapes and [`UnprocessableException`](./UnprocessableException.md) when the request is valid but the resource's *state* prevents the operation.
