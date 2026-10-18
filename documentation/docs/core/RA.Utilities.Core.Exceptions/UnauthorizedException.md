---
title: UnauthorizedException
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`UnauthorizedException` signals that authentication is missing or failed. The API layer maps it to **HTTP 401 Unauthorized**.

## 🎯 Purpose

Use this exception when the server cannot identify the caller: no token, an expired token, invalid credentials, or a failed signature check. The meaning is *"I don't know who you are."*

When the caller *is* authenticated but lacks permission, use [`ForbiddenException`](./ForbiddenException.md) (403) instead. Choosing the right one lets clients decide between re-authenticating and requesting more access.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `UnauthorizedException()` | `401` | `BaseResponseMessages.Unauthorized` (`"Authentication failed or is missing."`) |
| `UnauthorizedException(string message)` | `401` | supplied |
| `UnauthorizedException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.Unauthorized` when `null`/omitted |

The type always sets `ResponseType.Unauthorized`. The two-argument constructor is the only one that changes `ErrorCode`.

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `401` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.Unauthorized`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result> AuthenticateAsync(string? token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        // highlight-next-line
        return new UnauthorizedException("No authentication token was provided.");
    }

    if (!await _tokenValidator.IsValidAsync(token))
    {
        // highlight-next-line
        return new UnauthorizedException("The token is invalid or has expired.");
    }

    return Result.Success();
}
```

## 🔗 HTTP mapping

`ErrorResultResponse` recognizes `UnauthorizedException` and returns **HTTP 401** with an `UnauthorizedResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

Because the mapper selects the status from the exception type, overriding `ErrorCode` does not change the 401 status.

See [`UnauthorizedResponse`](../../api/RA.Utilities.Api.Response/UnauthorizedResponse.md) for the JSON shape, or [`ForbiddenException`](./ForbiddenException.md) when the caller is known but not permitted.
