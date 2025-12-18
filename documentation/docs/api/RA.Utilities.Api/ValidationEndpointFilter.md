---
title: ValidationEndpointFilter
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Api.EndpointFilters
```

The `ValidationEndpointFilter` is designed to seamlessly integrate [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) with your Minimal API endpoints, providing automatic request validation.

Here’s a step-by-step guide on how to set it up.

#### Step 1: Create a Validator for Your Request Model

First, you need a validator for the DTO (Data Transfer Object) or model you want to validate.
Using the [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) library, create a class that inherits from `AbstractValidator<T>`, where `T` is your request model.

For example, if you have a `CreateProductRequest` model, the validator would look like this:

```csharp
// Features/Products/CreateProduct.cs

using FluentValidation;

// The request model
public record CreateProductRequest(string Name, decimal Price, int Stock);

// The validator for the request model
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
```

#### Step 2: Register FluentValidation and Your Validators

Next, you need to register `FluentValidation` and all your validator classes with the dependency injection (DI) container in your `Program.cs` file.
The easiest way to do this is by using the `FluentValidation.AspNetCore` package, which provides the `AddValidatorsFromAssembly` extension method.

```csharp
// Program.cs

using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ... other services

// 1. Scan the specified assembly for any classes that inherit from 
//    AbstractValidator and register them with the DI container.
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ...
```

This single line will find the `CreateProductRequestValidator` (and any others) and make it available for the `ValidationEndpointFilter` to use.

#### Step 3: Apply the Filter to Your Minimal API Endpoint

Finally, apply the validation filter to your endpoint using the convenient `.Validate<TModel>()` extension method from the `RA.Utilities.Api.Extensions` namespace.

This shortcut method makes your endpoint registration cleaner and more readable.

Here’s how you would protect a "create product" endpoint:

```csharp
// In an IEndpoint implementation or Program.cs

using RA.Utilities.Api.Extensions;

// ...

app.MapPost("/products", (CreateProductRequest request) => 
{
    // This logic will only be executed if the request model is valid.
    // ... logic to create the product ...

    return Results.Created($"/products/123", request);
})
.Validate<CreateProductRequest>() // <-- Apply the validation filter here
.WithTags("Products");

// ...
```

### ⚙️ How It All Works Together

1. When a `POST` request is made to `/products`, the `ValidationEndpointFilter<CreateProductRequest>` intercepts it.
2. The filter finds the `CreateProductRequest` object from the endpoint's arguments.
3. It then asks the DI container for any registered validators for `CreateProductRequest` (which it finds because you registered them in `Program.cs`).
4. The validator's rules are executed against the incoming request body.
  * **If validation fails**, the filter short-circuits the request and automatically returns a standardized `400 Bad Request response`, complete with structured error details. Your endpoint's main logic is never called.
  * **If validation succeeds**, the filter calls `next()`, and the request proceeds to your endpoint handler as normal.

This pattern keeps your endpoint logic clean and focused on its core responsibility, while ensuring all incoming data is valid and all validation errors are handled consistently.
