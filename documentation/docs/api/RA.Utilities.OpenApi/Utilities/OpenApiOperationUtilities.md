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
They are particularly useful when you are creating your own custom `IOpenApiDocumentTransformer` or `IOpenApiOperationTransformer` and need to add dynamic or complex examples to certain endpoints.

### ✨ Key Functions:

- **`AddRequestExample(...)`**: Allows you to attach a named example to the request body of a specific API operation.
This is great for showing clients what a valid payload looks like.
- **`AddRequestExamples(...)`**: Adds multiple request examples to the specified OpenApi operation.
- **`AddResponseExample(...)`**: Allows you to attach a named example to a specific HTTP status code response for an operation.
You can use this to show what a `200 OK` response looks like, or what the body of a `404 Not Found` error contains.
- **`AddGeneralErrorResponse(...)`**: A specialized shortcut method that adds a pre-defined example for a `500 Internal Server Error`, saving you from having to construct it manually.
- **`AddResponseExamples(...)`**: Adds multiple response examples to the specified OpenAPI operation.

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
using RA.Utilities.Api.Response;
using RA.Utilities.OpenApi.Models;
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
using RA.Utilities.Api.Response;
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
| StatusCode | `int` | The HTTP status code for which the example is provided. |
| Summary | `string` | Short description for the example. |
| Description | `string` | Long description for the example. CommonMark syntax MAY be used for rich text representation. |
| Value | `object` | JSON value for Request or Response example. |
| MediaType | `string` | The media type of the example (***default is application/json***). |

:::caution[`MediaType` is ignored by `AddResponseExamples`]

`AddResponseExamples` forwards each example to `AddResponseExample` without the per-item `MediaType`, so batched response examples always use `application/json`. Call `AddResponseExample(..., mediaType: "application/problem+json")` directly when you need another media type.

:::

```csharp
using RA.Utilities.Api.Response;
using RA.Utilities.OpenApi.Models;
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
        OpenApiOperationUtilities.AddResponseExamples(operation, 
            new OpenApiResponseExample[] {
                new OpenApiResponseExample {
                    ExampleKey = "MinimumLengthValidator",
                    StatusCode = StatusCodes.Status400BadRequest,
                    Summary = "MinimumLengthValidator",
                    Description = "This is an example of a bad request due to a validation error.",
                    Value = new BadRequestResponse([
                        new() {
                                PropertyName = "Description",
                                ErrorMessage = "The length of 'Description' must be at least 5 characters. You entered 3 characters.",
                                AttemptedValue = "Buy",
                                ErrorCode = "MinimumLengthValidator"
                            }
                        ]
                    )
                },
                new OpenApiResponseExample {
                    ExampleKey = "GreaterThanOrEqualValidator",
                    StatusCode = StatusCodes.Status400BadRequest,
                    Summary = "Greater ThanOrEqual Validator",
                    Description = "This is an example of a bad request due to a validation error.",
                    Value = new BadRequestResponse([
                        new() {
                                PropertyName = "DueDate",
                                ErrorMessage = "'Due Date' must be greater than or equal to '17.9.2025 00:00:00'.",
                                AttemptedValue = "2025-09-15T20:29:19.36846+02:00",
                                ErrorCode = "GreaterThanOrEqualValidator"
                            }
                        ]
                    )
                }
            }
        );

        return Task.CompletedTask;
    });
}
```

### AddGeneralErrorResponse()

A convenience method that adds a pre-defined example for a `500 Internal Server Error` response. It uses `new ErrorResponse()` from `RA.Utilities.Api.Response` and stores the example under the `InternalServerError` key.

```csharp
public static void AddGeneralErrorResponse(OpenApiOperation operation);
```

## 🚀 Example Usage in a Custom Operation Transformer

A common use case is to implement `IOpenApiOperationTransformer` to add examples to specific endpoints. The transformer below adds a request example to an operation with the ID `CreateUser`.

```csharp showLineNumbers
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using RA.Utilities.OpenApi.Utilities;

public sealed class CreateUserExampleTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
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

        return Task.CompletedTask;
    }
}
```
