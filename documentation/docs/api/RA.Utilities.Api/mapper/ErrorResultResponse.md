```bash
Namespace: RA.Utilities.Api.Mapper
```

The `ErrorResultResponse` is a static helper class that provides a clean, centralized way to map exceptions from the
[`RA.Utilities.Core`](../../../core/UtilitiesCore/index.mdx) layer into standardized `IResult` objects for ASP.NET Core Minimal APIs.
It is a key component for creating consistent error responses when using the [`Result<T>`](../../ApiResults/index.mdx) pattern.

## 🎯 Purpose

When your application logic returns a [`Result<T>`](../../ApiResults/index.mdx) that is in a `Failure` state, it contains an `Exception` object
(e.g., [`NotFoundException`](../../../core/RA.Utilities.Core.Exceptions/NotFoundException.md), [`ConflictException`](../../../core/RA.Utilities.Core.Exceptions/ConflictException.md)). The `ErrorResultResponse.Result` method inspects this exception and automatically generates the appropriate HTTP error response (`404 Not Found`, `409 Conflict`, etc.) using the standardized models from `RA.Utilities.Api.Results`.

This allows you to handle all expected business-level failures in a single, declarative line of code within your API endpoints, keeping them clean and free of repetitive error-handling logic.

## 🚀 How to Use

The primary use case for `ErrorResultResponse` is within the `Match` method of a [`Result<T>`](../../ApiResults/index.mdx) object. The `ErrorResultResponse.Result` method has the signature `Func<Exception, IResult>`, which perfectly matches the signature required by the `failure` delegate of `Match`.

This allows you to pass the method directly, creating highly readable and maintainable endpoint code.

### Example

Imagine you have a service that retrieves a product and returns a `Result<Product>`. The result will be a `Failure` containing a [`NotFoundException`](../../../core/RA.Utilities.Core.Exceptions/NotFoundException.md) if the product doesn't exist.

In your Minimal API endpoint, you can use `ErrorResultResponse.Result` to handle this failure case automatically.

#### 1. The Service Layer returning a `Result`

```csharp showLineNumbers
// Application/Features/Products/ProductService.cs
using RA.Utilities.Core;
using RA.Utilities.Core.Exceptions;

public class ProductService
{
    public Result<Product> GetProductById(int id)
    {
        // Simulate not finding a product
        if (id != 1)
        {
            // Return a failure Result containing a NotFoundException
            return new NotFoundException(nameof(Product), id);
        }

        // Return a success Result with the product
        return new Product { Id = id, Name = "Sample Product" };
    }
}
```

#### 2. The API Endpoint using `Match` and `ErrorResultResponse`

```csharp
// highlight-next-line
using RA.Utilities.Api.Mapper

// Api/Features/Products/ProductEndpoints.cs
app.MapGet("/products/{id}", (int id, ProductService service) => 
{
    Result<Product> result = service.GetProductById(id);
    
    // The Match method handles both success and failure outcomes.
    // The `ErrorResultResponse.Result` method is passed directly to the failure delegate.
    // highlight-start
    return result.Match<IResult>(
        success: product => SuccessResponse.Ok(product),
        failure: ErrorResultResponse.Result 
    );
    // highlight-end
});
```

### What Happens

- If you request `/products/1`, the `result` is a success, and the `success` delegate is executed, returning a `200 OK` with the product data.
- If you request `/products/99`, the `result` is a failure containing a `NotFoundException`. The `failure` delegate (`ErrorResultResponse.Result`) is executed. It inspects the exception, sees it's a `NotFoundException`, and automatically returns a `404 Not Found` with a standardized JSON body:

```json showLineNumbers
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "Product with value '99' not found.",
  "result": {
    "entityName": "Product",
    "entityValue": 99
  }
}
```