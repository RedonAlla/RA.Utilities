---
title: NotFoundException
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `NotFoundException` is a semantic exception used to indicate that a requested resource could not be found.
It is designed to be translated into a standardized **HTTP 404 Not Found** response.

## 🎯 Purpose

This exception is used for predictable business-level failures when a lookup for a specific entity yields no result. Instead of returning `null` or throwing a generic exception, your business logic can return a `Failure` `Result` containing a `NotFoundException`.

This provides clear, structured information about the failure. The API layer is designed to catch this specific exception and automatically generate a 404 response, informing the client exactly what resource was missing.

## Properties

| Property      | Type     | Description                                                          |
|---------------|----------|----------------------------------------------------------------------|
| **EntityName**  | `string` | The name of the entity type that was not found (e.g., "Product").    |
| **EntityValue** | `object` | The value or identifier used to search for the entity (e.g., an ID). |

## 🚀 How to Use

You will typically create and return a `NotFoundException` from a service or handler when a database query or other lookup fails to find a required resource.

### Example: Fetching a Resource

In this example, a service attempts to retrieve a product by its ID. If the product doesn't exist, it returns a `Failure` `Result` with a `NotFoundException`.

```csharp showLineNumbers
// In a CQRS handler or application service
using RA.Utilities.Core;
// highlight-next-line
using RA.Utilities.Core.Exceptions;

public async Task<Result<ProductDto>> GetProductByIdAsync(int productId)
{
    // Attempt to find the product in the database
    var product = await _productRepository.FindByIdAsync(productId);

    if (product == null)
    {
        // Return a failure Result containing a NotFoundException
        // highlight-next-line
        return new NotFoundException(nameof(Product), productId);
    }

    // ... map product to ProductDto and return success
    var productDto = _mapper.Map<ProductDto>(product);
    return productDto; // Implicit success conversion
}
```

### Example JSON Output

When the API layer (using `ErrorResultResponse`) handles the `Failure` `Result` from the example above, it will automatically generate a `404 Not Found` response with a body like this:

```json showLineNumbers
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "Product with value '99' was not found.",
  "result": {
    "entityName": "Product",
    "entityValue": 99
  }
}
```

This structured response is invaluable for clients, as it programmatically confirms which entity and identifier could not be located.
