---
title: PaymentRequiredException
sidebar_position: 7
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`PaymentRequiredException` signals that the requested resource requires payment before it can be accessed. The API layer maps it to **HTTP 402 Payment Required**.

## 🎯 Purpose

Use this exception for paywalls and billing gates: a subscription that has not been purchased, an exhausted prepaid balance, or a quota that only a paid plan unlocks. The meaning is *"I know who you are, but this resource is not included in your current plan."*

Do not use it for missing or invalid credentials ([`UnauthorizedException`](./UnauthorizedException.md), 401), for a caller who is not allowed at all ([`ForbiddenException`](./ForbiddenException.md), 403), or for rate limiting ([`TooManyRequestsException`](./TooManyRequestsException.md), 429). A 402 tells the client that the request can succeed after payment.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `PaymentRequiredException()` | `402` | `BaseResponseMessages.PaymentRequired` (`"Payment is required to access this resource."`) |
| `PaymentRequiredException(string message)` | `402` | supplied |
| `PaymentRequiredException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.PaymentRequired` when `null`/omitted |

The type always sets `ResponseType.PaymentRequired`. The two-argument constructor is the only one that changes `ErrorCode`.

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `402` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.PaymentRequired`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result<Report>> GenerateReportAsync(Guid userId)
{
    Subscription subscription = await _subscriptions.GetAsync(userId);

    if (subscription.Plan == Plan.Free)
    {
        // highlight-next-line
        return new PaymentRequiredException("This report is available on paid plans.");
    }

    return await _reports.GenerateAsync(userId);
}
```

To attach a domain code while keeping the default message, pass `null`:

```csharp showLineNumbers
throw new PaymentRequiredException(errorCode: 4020);
```

## 🔗 HTTP mapping

`ErrorResultResponse` recognizes `PaymentRequiredException` and returns **HTTP 402** with a `PaymentRequiredResponse`. The envelope carries `responseCode` from `ErrorCode`, `responseType` as `PaymentRequired`, and `responseMessage` from `Message`; there is no nested `result` payload:

```json showLineNumbers
{
  "responseCode": 402,
  "responseType": 18,
  "responseMessage": "Payment is required to access this resource.",
  "result": null
}
```

With `JsonStringEnumConverter`, `responseType` is `"PaymentRequired"`. Because the mapper selects the status from the exception type, overriding `ErrorCode` does not change the 402 status.

See [`ErrorResultResponse`](../../api/RA.Utilities.Api/mapper/ErrorResultResponse.md) for how exception types are mapped to results.
