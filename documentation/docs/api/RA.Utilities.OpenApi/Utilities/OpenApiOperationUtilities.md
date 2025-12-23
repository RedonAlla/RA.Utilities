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

- **`AddRequestExample(...)`**: Allows you to attach a named example to the request body of a specific API operation.
This is great for showing clients what a valid payload looks like.
- **`AddRequestExamples(...)`**: Adds multiple request examples to the specified OpenApi operation.
- **`AddResponseExample(...)`**: Allows you to attach a named example to a specific HTTP status code response for an operation.
You can use this to show what a `200 OK` response looks like, or what the body of a `404 Not Found` error contains.
- **`AddGeneralErrorResponse(...)`**: A specialized shortcut method that adds a pre-defined example for a `500 Internal Server Error`, saving you from having to construct it manually.
- **`AddResponseExamples(...)`**: Adds multiple response examples to the specified OpenApi operation..

In short, this class provides the building blocks for creating richer, more illustrative API documentation beyond what is easily achievable with standard attributes.

## 🧩 Available Methods

### AddRequestExample()

Adds a named example to the request body of an operation for a given media type (defaults to `application/json`).

```csharp
using RA.Utilities.OpenApi.Utilities;

app.MapPost("todos", static async (
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken
    ) =>
        {
            //YOUR Logic
        })
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        OpenApiOperationUtilities.AddRequestExample(operation, "SimpleTodo", new OpenApiExample
        {
            Summary = "A simple todo item",
            Description = "This is an example of a minimal todo item, only providing the description.",
            Value = JsonSerializer.SerializeToNode(new CreateTodoRequest
            {
                Description = "Buy milk",
                IsCompleted = false
            })
        });

        return Task.CompletedTask;
    });
}
```

### AddRequestExamples()

Adds multiple request examples to the specified OpenApi operation.

#### OpenApiRequestExample Class
The `OpenApiRequestExample` class is a simple data structure used to pass a collection of examples to the `AddRequestExamples` method.

| Property | Type | Description |
| -------- | ---- | ----------- |
| ExampleKey | `string` | The key to identify the example. (e.g., "ValidUser", "UserNotFound"). |
| Summary | `string` | Short description for the example. |
| Description | `string` | Long description for the example. CommonMark syntax MAY be used for rich text representation. |
| Value | `object` | JSON value for Request or Response example. |
| MediaType | `string` | The media type of the example (***default is application/json***). |


```csharp
using RA.Utilities.OpenApi.Utilities;

app.MapPost("todos", static async (
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken
    ) =>
        {
            //YOUR Logic
        })
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        OpenApiOperationUtilities.AddRequestExamples(operation, 
            new OpenApiRequestExample[] {
                new OpenApiRequestExample {
                    ExampleKey = "SimpleTodo",
                    Summary = "A simple todo item",
                    Description = "This is an example of a minimal todo item, only providing the description.",
                    Value = new CreateTodoRequest
                    {
                        Description = "Buy milk",
                        IsCompleted = false
                    }
                },
                new OpenApiRequestExample {
                    ExampleKey = "TodoWithLabelsAndDueDate",
                    Summary = "A more complete todo item",
                    Description = "This is an example of a todo item with labels and a due date.",
                    Value = new CreateTodoRequest
                    {
                        Description = "Finish project report",
                        IsCompleted = false,
                        Labels = ["work", "urgent"],
                        DueDate = DateTime.Now.AddDays(3)
                    }
                }
            }
        );

        return Task.CompletedTask;
    });
}
```

### AddResponseExample()

Adds a named example to a specific status code response of an operation.

```csharp
using RA.Utilities.OpenApi.Utilities;

app.MapPost("todos", static async (
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken
    ) =>
        {
            //YOUR Logic
        })
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        OpenApiOperationUtilities.AddResponseExample(operation, StatusCodes.Status400BadRequest, "MinimumLengthValidator", new OpenApiExample
        {
            Summary = "MinimumLengthValidator",
            Description = "This is an example of a bad request due to a validation error.",
            Value = JsonSerializer.SerializeToNode(new BadRequestResponse([
                new() {
                        PropertyName = "Description",
                        ErrorMessage = "The length of 'Description' must be at least 5 characters. You entered 3 characters.",
                        AttemptedValue = "Buy",
                        ErrorCode = "MinimumLengthValidator"
                    }
                ]
            ))
        });

        return Task.CompletedTask;
    });
}
```

### AddResponseExamples
Adds multiple response examples to the specified OpenApi operation.

| Property | Type | Description |
| -------- | ---- | ----------- |
| ExampleKey | `string` | The key to identify the example. (e.g., "ValidUser", "UserNotFound"). |
| StatusCodes | `int` | The HTTP status code for which the example is provided. |
| Summary | `string` | Short description for the example. |
| Description | `string` | Long description for the example. CommonMark syntax MAY be used for rich text representation. |
| Value | `object` | JSON value for Request or Response example. |
| MediaType | `string` | The media type of the example (***default is application/json***). |


```csharp
using RA.Utilities.OpenApi.Utilities;

app.MapPost("todos", static async (
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken
    ) =>
        {
            //YOUR Logic
        })
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        OpenApiOperationUtilities.OpenApiResponseExample(operation, 
            new OpenApiResponseExample[] {
                new OpenApiResponseExample {
                    ExampleKey = "MinimumLengthValidator",
                    StatusCodes = StatusCodes.Status400BadRequest,
                    Summary = "MinimumLengthValidator",
                    Description = "This is an example of a bad request due to a validation error.",
                    Value = new CreateTodoRequest
                    {
                        PropertyName = "Description",
                        ErrorMessage = "The length of 'Description' must be at least 5 characters. You entered 3 characters.",
                        AttemptedValue = "Buy",
                        ErrorCode = "MinimumLengthValidator"
                    }
                },
                new OpenApiRequestExample {
                    ExampleKey = "GreaterThanOrEqualValidator",
                    StatusCodes = StatusCodes.Status400BadRequest,
                    Summary = "Greater ThanOrEqual Validator",
                    Description = "This is an example of a todo item with labels and a due date.",
                    Value = new CreateTodoRequest
                    {
                        PropertyName = "DueDate",
                        ErrorMessage = "'Due Date' must be greater than or equal to '17.9.2025 00:00:00'.",
                        AttemptedValue = "2025-09-15T20:29:19.36846+02:00",
                        ErrorCode = "GreaterThanOrEqualValidator"
                    }
                }
            }
        );

        return Task.CompletedTask;
    });
}
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
