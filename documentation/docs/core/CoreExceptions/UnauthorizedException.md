---
title: `UnauthorizedException`
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `UnauthorizedException` class is a semantic exception designed to represent a specific, predictable business-level failure: an authorization error.

Its primary purpose is to provide a clear, standardized way for your application and domain layers to signal that a user attempted an action they are not permitted to perform.

## Here’s a breakdown of its key roles:

#### 1. Semantic Clarity:
Instead of throwing a generic `Exception` with the message "Unauthorized," throwing an `UnauthorizedException` makes the code's intent explicit.
Anyone reading the code immediately understands the nature of the error.

#### 2. Standardized Error Code:
As seen in its constructors, it always calls the base constructor with `BaseResponseCode.Unauthorized` (which is 401). 
his hard-codes the link between this exception type and the HTTP 401 Unauthorized status code.

#### 3. Enables Automated API Responses: 
his is the most critical part. Your API's error handling infrastructure, specifically the `ErrorResultResponse` class, is built to recognize this specific exception type.

```csharp showLineNumbers
public static IResult Result(Exception exception) => exception switch
{
    // ... other cases
    // highlight-next-line
    UnauthorizedException baseException => Microsoft.AspNetCore.Http.Results.Json(
        data: ErrorResultMapper.MapToUnauthorizedResponse(baseException),
        statusCode: BaseResponseCode.Unauthorized
    ),
    // ... other cases
};
```

When `UnauthorizedException` is thrown anywhere in your application, the global exception handler catches it and uses this mapping logic to automatically generate a consistent, structured **HTTP 401 Unauthorized** response.

## ⚙️ How It's Used in Practice
This creates a clean separation of concerns:

* **Your Business Logic** doesn't need to know anything about HTTP.
It just needs to signal that an authorization rule was violated.

```csharp showLineNumbers
// In a service or feature handler
public Result UpdateProduct(int productId, User currentUser)
{
    var product = _productRepository.GetById(productId);

    // Business rule: Only the product's creator can update it.
    if (product.CreatedBy != currentUser.Id)
    {
        // Signal the failure. The API layer will handle the rest.
        return new UnauthorizedException("You are not permitted to update this product.");
    }

    // ... proceed with update logic
    return Result.Success();
}
```
* **Your API Layer** doesn't need to know the specific business rules.
It just knows how to translate an `UnauthorizedException` into the correct HTTP response.

## 🧠 Summary
In summary, `UnauthorizedException` acts as a contract between your business logic and your API layer, enabling robust, predictable, and maintainable error handling for authorization failures.