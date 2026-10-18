---
title: NotFoundException
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`NotFoundException` signals that a requested resource does not exist. The API layer maps it to **HTTP 404 Not Found**.

## 🎯 Purpose

Use this exception for predictable lookups that return nothing. Instead of returning `null` or throwing a generic exception, the business layer throws (or returns) `NotFoundException`, and the API layer produces a consistent 404 response.

Two shapes are available:

- **Entity-aware** — `new NotFoundException("Product", 99)` populates `EntityName` and `EntityValue` and generates `"Product with value '99' was not found."`
- **Generic** — `new NotFoundException()` (or `new NotFoundException(errorCode, message)`) produces a 404 without disclosing an entity.

## Properties

| Property | Type | Description |
|---|---|---|
| **`EntityName`** | `string` | Name of the entity that was not found (e.g., `"Product"`). Set only by the entity/value constructors. |
| **`EntityValue`** | `object` | Identifier used in the lookup (e.g., `99`, `"a@b.com"`). Set only by the entity/value constructors. |
| `ErrorCode` | `int` | Inherited. `404` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.NotFound`. |
| `Message` | `string` | Inherited. The supplied message when one is given, the entity template for entity/value constructors, otherwise `BaseResponseMessages.NotFound`. |

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `NotFoundException()` | `404` | `BaseResponseMessages.NotFound` |
| `NotFoundException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.NotFound` when `null`/omitted |
| `NotFoundException(string entity, object value)` | `404` | `"{entity} with value '{value}' was not found."` |
| `NotFoundException(string entity, object value, string? message)` | `404` | supplied, or the entity template when `null` |
| `NotFoundException(string entity, object value, int errorCode, string? message = null)` | supplied | supplied, or the entity template when `null`/omitted |

The entity/value constructors set `EntityName` and `EntityValue`; the others leave both unset.

## 🚀 How to Use

### Example: Fetching a resource

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result<ProductDto>> GetProductByIdAsync(int productId)
{
    Product? product = await _productRepository.FindByIdAsync(productId);

    if (product is null)
    {
        // highlight-next-line
        return new NotFoundException(nameof(Product), productId);
    }

    var productDto = _mapper.Map<ProductDto>(product);
    return productDto; // Implicit success conversion
}
```

The resulting exception has `ErrorCode = 404`, `ResponseType = ResponseType.NotFound`, `EntityName = "Product"`, `EntityValue = 99`, and `Message = "Product with value '99' was not found."`.

### Example: A generic 404

Use the parameterless constructor when the response should not disclose an entity, or pass a custom code when your clients need one:

```csharp showLineNumbers
// 404 / ResponseType.NotFound / BaseResponseMessages.NotFound
return new NotFoundException();

// 404 / custom code / default message
return new NotFoundException(errorCode: 4040);

// 404 / custom code / custom message
return new NotFoundException(4040, "The catalog item is no longer available.");
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `NotFoundException` to **HTTP 404** with a `NotFoundResponse` body containing `entity` and `value` from the exception. The `ErrorCode` is echoed into `responseCode`; it does not change the HTTP status.

:::note[Known nested-payload issue]

`ErrorResultMapper.ToResponse(NotFoundException)` currently passes the exception message as the `errorCode` argument of `NotFoundResult`, so the nested `errorCode` contains the message and `errorMessage` is empty. The envelope (`responseCode`, `responseType`, `responseMessage`) is correct. See [`ErrorResultResponse`](../../api/RA.Utilities.Api/mapper/ErrorResultResponse.md) for the current output.

:::

See also [`ConflictException`](./ConflictException.md) for uniqueness conflicts and [`Troubleshooting`](./troubleshooting.mdx) for common questions.
