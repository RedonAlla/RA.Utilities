---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The primary purpose of an `IRequestHandler` is to contain the business logic for a single, specific feature (a command or a query).
When a message (an `IRequest`) is sent through the mediator pipeline, the mediator finds the corresponding `IRequestHandler` and invokes its `HandleAsync` method to process the request.

This pattern ensures that the logic for each use case is completely isolated in its own handler class, making the system highly maintainable and easy to understand.

`RA.Utilities.Feature` defines two versions of this interfaces

* **`IRequestHandler<TRequest, TResponse>`**:
This is the most common version.
It's for handlers that process a request and are expected to return a value.
The `HandleAsync` method returns a `Task<TResponse>` directly — no `Result` wrapper. Errors are communicated by **throwing** typed exceptions from `RA.Utilities.Core.Exceptions` (`NotFoundException`, `ConflictException`, `BadRequestException`, ...), which the API layer's `GlobalExceptionHandler` maps to HTTP responses.

* **`IRequestHandler<in TRequest>`**:
This version is for handlers that process a request but do not return a value (often called "fire-and-forget" operations).
Its `HandleAsync` method returns a plain `Task`.

Both interfaces also declare a context-aware `HandleAsync<TContext>(...)` overload that receives a [`PipelineContext<TContext>`](../Models/PipelineContext.md). Its default implementation delegates to the plain overload, so you only override it when you need context data.

## 🔑 Key characteristics:

| Feature              | Description                                                                                                  |
| -------------------- | ------------------------------------------------------------------------------------------------------------ |
| **Pattern**          | Request → Response (1-to-1)                                                                                  |
| **Interface**        | `IRequestHandler<TRequest, TResponse>`                                                                       |
| **Return value**     | Returns a single result (`TResponse`)                                                                        |
| **Mediator method**  | `Send()`                                                                                                     |
| **Typical use case** | Fetching data, executing a command, performing business logic where a single handler must handle the request |

## 🚀 Usage Example

Let's walk through creating a complete feature slice for creating a new product.

### Step 1: Define the Command

First, define the command (the request) and its validation rules.

```csharp
// Features/Products/CreateProduct.cs
using RA.Utilities.Feature.Abstractions;

// The command containing the data for the new product.
// TResponse is the plain return type of the handler — no Result wrapper.
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
```

### Step 2: Implement the Handler

Next, create the handler by inheriting from [`RequestHandler<TRequest, TResponse>`](../Handlers/RequestHandler.md). Just override `HandleAsync` and return the value directly; report failures by throwing typed exceptions.

```csharp
// Features/Products/CreateProduct.cs (continued)
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Feature.Handlers;

public class CreateProductHandler : RequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public override async Task<int> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (await _productRepository.DoesProductExistAsync(command.Name))
        {
            throw new ConflictException(nameof(Product), command.Name);
        }

        var newProduct = new Product { Name = command.Name, Price = command.Price };
        var productId = await _productRepository.AddAsync(newProduct);

        return productId;
    }
}
```

### Step 3: Register Services in `Program.cs`

Handlers implementing `IRequestHandler<,>` are discovered by the source generator, so `AddMediator()` registers them automatically. Only behaviors and validators need explicit registration:

```csharp
// Program.cs
using RA.Utilities.Feature.Behaviors;
using RA.Utilities.Feature.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator(); // registers IMediator + all handler implementations

builder.Services
    .AddFeature<CreateProductCommand, int, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>();

var app = builder.Build();

// ... your endpoint mapping

app.Run();
```

`AddFeature` also re-registers the handler (same scoped registration), so it is only needed here to reach the builder for `.AddValidator` / `.AddDecoration`.
