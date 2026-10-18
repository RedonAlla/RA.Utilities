---
title: GatewayTimeoutException
sidebar_position: 11
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`GatewayTimeoutException` signals that an upstream server or dependency did not respond in time. The API layer maps it to **HTTP 504 Gateway Timeout**.

## 🎯 Purpose

Use this exception when this service, acting as a gateway or proxy, waited for an upstream response and gave up. Common causes:

- A payment gateway that times out during processing.
- An external API call that exceeds its configured timeout.
- A database query that runs longer than allowed.
- A message broker or queue operation that does not complete.

Do not use it for client cancellations. Check `CancellationToken.IsCancellationRequested` and let the request abort when the caller disconnected.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `GatewayTimeoutException()` | `504` | `BaseResponseMessages.GatewayTimeout` (`"The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server."`) |
| `GatewayTimeoutException(string message)` | `504` | supplied |
| `GatewayTimeoutException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.GatewayTimeout` when `null`/omitted |

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `504` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.GatewayTimeout`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

### Example: Payment gateway timeout

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result<Order>> ProcessPaymentAsync(Guid orderId)
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    try
    {
        return await _paymentGateway.ProcessAsync(orderId, cts.Token);
    }
    catch (TaskCanceledException) when (!cts.IsCancellationRequested)
    {
        // highlight-next-line
        return new GatewayTimeoutException(
            "The payment gateway did not respond in time. Please try again later."
        );
    }
}
```

### Example: External API timeout

```csharp showLineNumbers
catch (TaskCanceledException)
{
    // highlight-next-line
    return new GatewayTimeoutException("The external data provider did not respond in time.");
}
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `GatewayTimeoutException` to **HTTP 504** with a `GatewayTimeoutResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

See [`GatewayTimeoutResponse`](../../api/RA.Utilities.Api.Response/GatewayTimeoutResponse.md) for the JSON shape.
