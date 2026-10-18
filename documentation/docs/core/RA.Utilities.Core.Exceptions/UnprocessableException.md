---
title: UnprocessableException
sidebar_position: 8
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`UnprocessableException` signals that a request is syntactically valid but cannot be processed because of the resource's current state. The API layer maps it to **HTTP 422 Unprocessable Entity**.

## 🎯 Purpose

This exception represents a *state conflict*. The client sent well-formed data, but the server's current state makes the operation impossible.

Common scenarios include:

- Deleting a resource that is currently in use.
- An invalid status transition (for example, cancelling a shipped order).
- Modifying a locked or archived record.
- A business-rule limit (withdrawing more than the daily limit).

Use [`BadRequestException`](./BadRequestException.md) when the input itself is invalid, and [`ConflictException`](./ConflictException.md) when the conflict is a uniqueness or concurrency violation.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `UnprocessableException()` | `422` | `BaseResponseMessages.Unprocessable` (`"Unprocessable entity."`) |
| `UnprocessableException(string message)` | `422` | supplied |
| `UnprocessableException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.Unprocessable` when `null`/omitted |

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `422` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.Unprocessable`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

### Example: Invalid status transition

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result> CancelOrderAsync(Guid orderId)
{
    Order? order = await _orderRepository.GetByIdAsync(orderId);

    if (order is null)
    {
        return new NotFoundException(nameof(Order), orderId);
    }

    if (order.Status == OrderStatus.Shipped)
    {
        // highlight-next-line
        return new UnprocessableException("Cannot cancel an order that has already been shipped.");
    }

    order.Status = OrderStatus.Cancelled;
    await _orderRepository.UpdateAsync(order);
    return Result.Success();
}
```

### Example: A custom code with the default message

```csharp showLineNumbers
throw new UnprocessableException(errorCode: 4221);
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `UnprocessableException` to **HTTP 422** with an `UnprocessableResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

See [`UnprocessableResponse`](../../api/RA.Utilities.Api.Response/UnprocessableResponse.md) for the JSON shape.
