```bash
Namespace: RA.Utilities.Api.Mapper
```

The `SuccessResponse` static class, located in the `RA.Utilities.Api` package, is a set of helper methods designed to streamline the creation of standardized, successful API responses.

Its primary goals are:

**1. Enforce Consistency**:
It ensures that every successful response from your API (like a `200 OK` or `201 Created`) is wrapped in the standard `SuccessResponse<T>` model from the `RA.Utilities.Api.Results` package.
This provides a predictable JSON structure (`responseCode`, `responseType`, `result`, etc.) for all your API's success outcomes, which is a mirror to how errors are handled.

**2. Reduce Boilerplate**:
It provides simple, expressive static methods that are much cleaner to use than manually constructing the response.
For example, writing return `SuccessResponse.Ok(product);` is more concise and readable than `return Results.Ok(new SuccessResponse<Product>(product));`.

**3. Simplify HTTP Compliance**:
It offers convenient overloads for common HTTP success statuses (`200`, `201`, `202`, `204`), handling details like setting the `Location` header for `201 Created` responses automatically.

In short, this class is a key part of the "batteries-included" developer experience, helping you write clean, consistent, and maintainable API endpoints with minimal effort.

## Available Helpers
The SuccessResponse class provides helpers for the most common success status codes:

* `Ok()`: Creates a `200 OK` response.
* `Ok<T>(T result)`: Creates a `200 OK` response with a payload.
* `Created()`: Creates a `201 Created` response.
* `Created<T>(T result)`: Creates a `201 Created` response with a payload.
* `Created<T>(string uri, T result)`: Creates a `201 Created` response with a `Location` header and a payload.
* `Accepted()`: Creates a `202 Accepted` response.
* `Accepted<T>(string uri, T result)`: Creates a `202 Accepted` response with a `Location` header and a payload.
* `NoContent()`: Creates a `204 No Content` response.

## 🚀 Usage Guide
You can use these helpers directly in your endpoints to simplify response creation and ensure consistency.

```csharp
// highlight-next-line
using RA.Utilities.Api.Mapper;

// In an IEndpoint implementation or Minimal API
app.MapGet("/products/{id}", (int id) => 
{
    var product = new Product { Id = id, Name = "Sample Product" };

    // Instead of this:
    // return Results.Ok(new SuccessResponse<Product>(product));

    // You can write this:
    // highlight-next-line
    return SuccessResponse.Ok(product);
});

app.MapPost("/products", (Product product) => 
{
    // Logic to save the product...
    var newProduct = new Product { Id = 123, Name = product.Name };

    // Creates a 201 Created response with a Location header and a structured body
    // highlight-next-line
    return SuccessResponse.Created($"/products/{newProduct.Id}", newProduct);
});
```

A request that returns a product would yield the following JSON body, wrapped in the standard response structure:

```json showLineNumbers
{
  "responseCode": 200,
  "responseType": "Success",
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 1,
    "name": "Sample Product"
  }
}
```

### Using the `Result` Type with Endpoints

#### Step 1: Create a service that returns a `Result`
Your application or domain layer should return a `Result<T>` to indicate the outcome of an operation.

```csharp showLineNumbers
// Services/ProductService.cs
using RA.Utilities.Core;
using RA.Utilities.Core.Exceptions;

public class ProductService
{
    public Result<Product> GetProductById(int id)
    {
        if (id <= 0)
        {
            // Return a failure Result for invalid input
            return new BadRequestException("Invalid product ID.");
        }

        if (id != 1)
        {
            // Return a failure Result if the product is not found
            return new NotFoundException(nameof(Product), id);
        }

        // Return a success Result with the product
        return new Product { Id = id, Name = "Sample Product" };
    }
}
```

#### Step 2: Use `Match` in your endpoint to return an `IResult`

In your `IEndpoint` implementation, call the service and use the `Match` method to map the `Result` to an HTTP response. This pattern forces you to handle both success and failure cases explicitly.

```csharp showLineNumbers
// Features/Products/ProductEndpoints.cs

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id}", (int id, ProductService service) => 
        {
            Result<Product> result = service.GetProductById(id);
            
            // Match the result to an appropriate HTTP response
            // highlight-start
            return result.Match<IResult>(
                // Use the new helper for a clean success response
                success: product => SuccessResponse.Ok(product),
                failure: ErrorResultResponse.Result // Use the error mapper for a consistent failure response
            );
            // highlight-end

            // OR Short
            // highlight-next-line
            //return result.Match<IResult>(SuccessResponse.Ok(product), ErrorResultResponse.Result);
        }).WithTags("Products");
    }
}
```