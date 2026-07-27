---
title: GatewayTimeoutException
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `GatewayTimeoutException` is a semantic exception used to signal that an upstream server or external dependency did not respond within the expected time.
It is designed to be translated into a standardized **HTTP 504 Gateway Timeout** response.

## 🎯 Purpose

This exception represents a timeout scenario where the server, acting as a gateway or proxy, did not receive a timely response from an upstream server. Common scenarios include:

- A payment gateway that times out during processing
- An external API call that exceeds its configured timeout
- A database query that runs longer than the allowed duration
- A message broker or queue operation that doesn't complete in time

By throwing a `GatewayTimeoutException`, your business logic clearly communicates the nature of the failure to the API layer, which can then generate an appropriate 504 response.

## Constructors

| Constructor | Description |
|---|---|
| `GatewayTimeoutException()` | Uses default error code (`504`) and default message. |
| `GatewayTimeoutException(string message)` | Uses default error code (`504`) with a custom message. |
| `GatewayTimeoutException(int errorCode, string message = ...)` | Custom error code with an optional custom message. |

## 🚀 How to Use

### Example: Payment Gateway Timeout

```csharp showLineNumbers
using RA.Utilities.Core;
// highlight-next-line
using RA.Utilities.Core.Exceptions;

public async Task<Result<Order>> ProcessPaymentAsync(Guid orderId)
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    try
    {
        var paymentResult = await _paymentGateway.ProcessAsync(orderId, cts.Token);
        return paymentResult;
    }
    catch (TaskCanceledException)
    {
        // Return a failure Result with a GatewayTimeoutException
        // highlight-next-line
        return new GatewayTimeoutException(
            errorCode: 504,
            message: "The payment gateway did not respond in time. Please try again later."
        );
    }
}
```

### Example: Generic Upstream Failure

```csharp showLineNumbers
public async Task<Result<Data>> FetchFromExternalApiAsync()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

    try
    {
        var response = await _httpClient.GetAsync("/external/data", cts.Token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Data>();
    }
    catch (TaskCanceledException)
    {
        // highlight-next-line
        return new GatewayTimeoutException("The external data provider did not respond in time.");
    }
    catch (OperationCanceledException)
    {
        // highlight-next-line
        return new GatewayTimeoutException("The upstream service timed out.");
    }
}
```

### Example JSON Output

When the API layer (using `ErrorResultResponse`) handles the `Failure` `Result` from the examples above, it will automatically generate a `504 Gateway Timeout` response:

```json showLineNumbers
{
  "responseCode": 504,
  "responseType": "GatewayTimeout",
  "responseMessage": "The payment gateway did not respond in time. Please try again later.",
  "result": {
    "errorCode": 504,
    "errorMessage": "The payment gateway did not respond in time. Please try again later."
  }
}
```
