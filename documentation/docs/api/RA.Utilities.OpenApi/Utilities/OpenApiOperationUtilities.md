---
title: OpenApiOperationUtilities
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.OpenApi.Utilities
```

The `OpenApiOperationUtilities` class is a static helper that provides methods for programmatically adding examples to requests and responses within an `OpenApiOperation`.

### 🎯 Purpose

The `OpenApiOperationUtilities` class is a static helper class that provides convenient methods for programmatically modifying `OpenApiOperation` objects within the OpenAPI generation pipeline.
Its primary purpose is to simplify the process of adding request and response examples to your API documentation.

While attributes can define schemas, adding specific, named examples often requires more direct manipulation of the OpenAPI document.
This is where these utilities shine.
They are particularly useful when you are creating your own custom `IOpenApiDocumentTransformer` or `IOperationFilter` and need to add dynamic or complex examples to certain endpoints.

### ✨ Key Functions:

1. **`AddRequestExample(...)`**: Allows you to attach a named example to the request body of a specific API operation.
This is great for showing clients what a valid payload looks like.
2. **`AddResponseExample(...)`**: Allows you to attach a named example to a specific HTTP status code response for an operation.
You can use this to show what a `200 OK` response looks like, or what the body of a `404 Not Found` error contains.
3. **`AddGeneralErrorResponse(...)`**: A specialized shortcut method that adds a pre-defined example for a `500 Internal Server Error`, saving you from having to construct it manually.

In short, this class provides the building blocks for creating richer, more illustrative API documentation beyond what is easily achievable with standard attributes.

## 🧩 Available Methods

### AddRequestExample()

Adds a named example to the request body of an operation for a given media type (defaults to `application/json`).

```csharp showLineNumbers
public static void AddRequestExample(
    OpenApiOperation operation,
    string exampleName,
    IOpenApiExample example,
    string mediaType = "application/json"
);
```

### AddResponseExample()

Adds a named example to a specific status code response of an operation.

```csharp showLineNumbers
public static void AddResponseExample(
    OpenApiOperation operation,
    int statusCode,
    string exampleName,
    IOpenApiExample example,
    string mediaType = "application/json"
);
```

### AddGeneralErrorResponse()

A convenience method that adds a pre-defined example for a `500 Internal Server Error` response.

```csharp
public static void AddGeneralErrorResponse(OpenApiOperation operation);
```

## 🚀 Example Usage in an `IOperationFilter`

A common use case is to create an `IOperationFilter` to add examples to specific endpoints. The filter below adds a request example to an operation with the ID `CreateUser`.

```csharp showLineNumbers
using System.Text.Json;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.Utilities;

public class CreateUserExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Target a specific operation by its ID
        if (operation.OperationId == "CreateUser")
        {
            var example = new OpenApiExample
            {
                Summary = "Example of a new user payload",
                Value = JsonSerializer.SerializeToNode(new
                {
                    firstName = "John",
                    lastName = "Doe",
                    email = "john.doe@example.com"
                })
            };

            // Use the utility to add the example
            // highlight-next-line
            OpenApiOperationUtilities.AddRequestExample(operation, "NewUser", example);
        }
    }
}
```
