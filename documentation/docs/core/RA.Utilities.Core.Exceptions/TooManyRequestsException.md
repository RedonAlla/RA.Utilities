---
title: TooManyRequestsException
sidebar_position: 9
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`TooManyRequestsException` signals that the caller exceeded a rate limit or throttling threshold. The API layer maps it to **HTTP 429 Too Many Requests**.

## 🎯 Purpose

Use this exception when a client sends too many requests in a time window. Typical sources are per-minute API limits, brute-force protection, tenant quotas, and traffic anomalies.

Because the condition is temporary, include enough context for the client to back off — and send a `Retry-After` header from your endpoint or middleware when you know the window.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `TooManyRequestsException()` | `429` | `BaseResponseMessages.TooManyRequests` (`"Too many requests. Please try again later."`) |
| `TooManyRequestsException(string message)` | `429` | supplied |
| `TooManyRequestsException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.TooManyRequests` when `null`/omitted |

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `429` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.TooManyRequests`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

### Example: Per-client rate limit

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result> EnforceRateLimitAsync(string clientId)
{
    int requestCount = await _rateLimitStore.GetCountAsync(clientId);

    if (requestCount >= _maxRequestsPerMinute)
    {
        // highlight-next-line
        return new TooManyRequestsException(
            $"Rate limit exceeded. Maximum {_maxRequestsPerMinute} requests per minute allowed."
        );
    }

    await _rateLimitStore.IncrementAsync(clientId);
    return Result.Success();
}
```

### Example: Tenant quota

```csharp showLineNumbers
if (usage.ApiCalls >= usage.PlanLimit)
{
    // highlight-next-line
    return new TooManyRequestsException(
        "You have exceeded your plan's monthly API call limit. Please upgrade your plan."
    );
}
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `TooManyRequestsException` to **HTTP 429** with a `TooManyRequestsResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

See [`TooManyRequestsResponse`](../../api/RA.Utilities.Api.Response/TooManyRequestsResponse.md) for the JSON shape.
