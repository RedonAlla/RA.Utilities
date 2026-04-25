---
title: UnprocessableException
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `UnprocessableException` is a semantic exception used to signal that a requested action cannot be performed because of the current state of a resource.
It is designed to be translated into a standardized **HTTP 422 Unprocessable Entity** response. 

## 🎯 Purpose
This exception represents a "State Conflict."
It is used when the client's request is technically valid (correct format and types), but the server's current data state makes the operation impossible.

Common scenarios include:
- Attempting to delete a resource that is currently "In Use."
- Attempting to transition an entity to a status that is not allowed from its current status (e.g., Shipped → Cancelled).
- Modifying a record that has been "Locked" or "Archived."
- Trying to withdraw $100 when the daily limit is $50 (Business Rule violation).

## 🚀 How to Use
Throw this exception from your domain services or application handlers when a business rule transition is violated.

### Example: Invalid Status Transition

```csharp
public async Task<Result> CancelOrderAsync(Guid orderId)
{
    var order = await _orderRepository.GetByIdAsync(orderId);

    if (order == null)
    {
        return new NotFoundException(nameof(Order), orderId);
    }

    // Business Rule: Shipped orders cannot be cancelled.
    if (order.Status == OrderStatus.Shipped)
    {
        return new UnprocessableException(
            "ORDER_ALREADY_SHIPPED",
            "Cannot cancel an order that has already been shipped."
        );
    }

    order.Status = OrderStatus.Cancelled;
    await _orderRepository.UpdateAsync(order);
    return Result.Success();
}
```

### Example JSON Output
When the API layer handles a Failure Result containing an `UnprocessableException`, it will automatically generate a 422 response with a body like this:

```json
{
    "responseCode": 422,
    "responseType": "Unprocessable",
    "responseMessage": "Cannot cancel an order that has already been shipped.",
    "result": {
        "errorCode": "ORDER_ALREADY_SHIPPED",
        "message": "Cannot cancel an order that has already been shipped."
    }
}
```
