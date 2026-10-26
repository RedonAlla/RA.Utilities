---
title: ForbiddenException
sidebar_position: 8
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`ForbiddenException` signals that an authenticated caller is not allowed to perform the requested action. The API layer maps it to **HTTP 403 Forbidden**.

## 🎯 Purpose

Use this exception when the server knows who the caller is but the caller lacks the required role, claim, ownership, or permission. The meaning is *"I know who you are, but you can't do this."*

Do not use it for missing or invalid credentials — that is [`UnauthorizedException`](./UnauthorizedException.md) (401).

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `ForbiddenException()` | `403` | `BaseResponseMessages.Forbidden` (`"You do not have permission to access this resource."`) |
| `ForbiddenException(string message)` | `403` | supplied |
| `ForbiddenException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.Forbidden` when `null`/omitted |

The type always sets `ResponseType.Forbidden`. The two-argument constructor is the only one that changes `ErrorCode`.

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `403` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.Forbidden`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result> UpdateSensitiveDataAsync(Guid resourceId, User currentUser)
{
    Resource? resource = await _repository.GetByIdAsync(resourceId);

    // Business rule: only the owner or a super admin may edit this resource.
    if (resource.OwnerId != currentUser.Id && !currentUser.IsSuperAdmin)
    {
        // highlight-next-line
        return new ForbiddenException("You do not have permission to modify this resource.");
    }

    // ... proceed with the update
    return Result.Success();
}
```

To attach a domain code while keeping the default message, pass `null`:

```csharp showLineNumbers
throw new ForbiddenException(errorCode: 4030);
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `ForbiddenException` to **HTTP 403** with a `ForbiddenResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

See [`ForbiddenResponse`](../../api/RA.Utilities.Api.Response/ForbiddenResponse.md) for the JSON shape.
