```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `EndpointBuilderExtensions` class is designed to improve the developer experience when applying validation to Minimal API endpoints. It provides a more fluent and readable syntax for adding the `ValidationEndpointFilter`.

Instead of the more verbose `AddEndpointFilter<ValidationEndpointFilter<TModel>>`, this class provides a simple and elegant shortcut.

## `Validate<TModel>()`

This extension method for `RouteHandlerBuilder` adds an endpoint filter that automatically validates a model of type `TModel` using `FluentValidation`. If validation fails, the request is short-circuited, and a standardized validation error is returned before the endpoint handler is ever executed.

On failure, the filter returns an [**HTTP `400 Bad Request`**](../../RA.Utilities.Api.Results/BadRequestResponse.md) with a structured JSON body containing the validation errors, providing a consistent and predictable error-handling experience for your API consumers.

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "One or more validation errors occurred.",
  "result": [
    {
      "propertyName": "Email",
      "errorMessage": "'Email' must not be empty.",
      "attemptedValue": "",
      "errorCode": "NotEmptyValidator"
    }
  ]
}
```

### 🚀 Usage

By chaining the `Validate<TModel>()` method to your endpoint definition, you can make your code more concise and readable.

Consider the following example where we want to validate a `CreateProductRequest` model.

#### Before

Without the extension, you would need to add the filter manually:

```csharp
app.MapPost("/products", (CreateProductRequest request) => { ... })
   .AddEndpointFilter<ValidationEndpointFilter<CreateProductRequest>>();
```

#### After

With the `Validate<TModel>()` extension method, the same registration becomes much cleaner:

```csharp
app.MapPost("/products", (CreateProductRequest request) => { ... })
   .Validate<CreateProductRequest>();
```