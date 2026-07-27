---
title: TooManyRequestsException
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `TooManyRequestsException` is a semantic exception used to signal that the client has exceeded a rate limit or throttling threshold.
It is designed to be translated into a standardized **HTTP 429 Too Many Requests** response.

## 🎯 Purpose

This exception represents a rate limiting or throttling scenario where the client has sent too many requests in a given time window. Common scenarios include:

- A client exceeding a per-minute API call limit
- A user hitting a brute-force protection threshold
- A tenant exceeding their plan's request quota
- DDoS protection triggering on abnormal traffic patterns

By throwing a `TooManyRequestsException`, your business logic clearly communicates the rate limit violation to the API layer, which can then generate an appropriate 429 response — often with a `Retry-After` header.

## Constructors

| Constructor | Description |
|---|---|
| `TooManyRequestsException()` | Uses default error code (`429`) and default message. |
| `TooManyRequestsException(string message)` | Uses default error code (`429`) with a custom message. |
| `TooManyRequestsException(int errorCode, string message = ...)` | Custom error code with an optional custom message. |

## 🚀 How to Use

### Example: Rate Limiting Middleware

```csharp showLineNumbers
using RA.Utilities.Core;
// highlight-next-line
using RA.Utilities.Core.Exceptions;

public async Task<Result> EnforceRateLimitAsync(string clientId)
{
    var requestCount = await _rateLimitStore.GetCountAsync(clientId);

    if (requestCount >= _maxRequestsPerMinute)
    {
        // highlight-next-line
        return new TooManyRequestsException(
            errorCode: BaseResponseCode.TooManyRequests,
            message: $"Rate limit exceeded. Maximum {_maxRequestsPerMinute} requests per minute allowed."
        );
    }

    await _rateLimitStore.IncrementAsync(clientId);
    return Result.Success();
}
```

### Example: Quota Enforcement

```csharp showLineNumbers
public async Task<Result> CheckTenantQuotaAsync(Guid tenantId)
{
    var usage = await _quotaService.GetMonthlyUsageAsync(tenantId);

    if (usage.ApiCalls >= usage.PlanLimit)
    {
        // highlight-next-line
        return new TooManyRequestsException(
            "You have exceeded your plan's monthly API call limit. Please upgrade your plan."
        );
    }

    return Result.Success();
}
```

### Example JSON Output

When the API layer handles a `Failure` `Result` containing a `TooManyRequestsException`, it will generate a `429 Too Many Requests` response:

```json showLineNumbers
{
  "responseCode": 429,
  "responseType": "TooManyRequests",
  "responseMessage": "Rate limit exceeded. Maximum 100 requests per minute allowed.",
  "result": {
    "errorCode": 429,
    "errorMessage": "Rate limit exceeded. Maximum 100 requests per minute allowed."
  }
}
```
