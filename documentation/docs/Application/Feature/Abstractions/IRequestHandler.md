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

* **`IRequestHandler<TRequest, TResponse>`:
This is the most common version.
It's for handlers that process a request and are expected to return a value.
The `HandleAsync` method returns a `Task<Result<TResponse>>`, wrapping the outcome in the `Result` type from your `RA.Utilities.Core` package for robust success/failure handling.

* **`IRequestHandler<in TRequest>`**:
This version is for handlers that process a request but do not return a value (often called "fire-and-forget" operations).
Its `HandleAsync` method returns a `Task<Result>`, which still allows you to know if the operation succeeded or failed, but without a return value.

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
using RA.Utilities.Core;

// The command containing the data for the new product
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<int>>;
```

### Step 2: Implement the Handler

Next, create the handler by inheriting from `IRequestHandler<TRequest, TResponse>`. This is where your business logic lives.

```csharp
// Features/Products/CreateProduct.cs (continued)
// highlight-next-line
using RA.Utilities.Feature.Abstractions;
using Microsoft.Extensions.Logging;
using RA.Utilities.Core;

public class CreateProductHandler : RequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IProductRepository _productRepository;
 
    // Inject dependencies and the base logger
    public CreateProductHandler(IProductRepository productRepository, ILogger<CreateProductHandler> logger)
        : base(logger)
    {
        _productRepository = productRepository;
    }

    // Override the base HandleAsync to implement the core business logic
    public override async Task<Result<int>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Check if a product with the same name already exists
        if (await _productRepository.DoesProductExistAsync(command.Name))
        {
            // Return a failure Result using a custom exception
            return new ConflictException(nameof(Product), command.Name);
        }

        var newProduct = new Product { Name = command.Name, Price = command.Price };
        
        var productId = await _productRepository.AddAsync(newProduct);

        // Return a success Result with the new product's ID
        return productId;
    }
}
```

### Step 3: Register Services in `Program.cs` //TODO add Registration documentation link

Finally, wire up MediatR, the validation behavior, and your validators in your application's service configuration.

```csharp
// Program.cs
// highlight-next-line
using RA.Utilities.Feature.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
_ = services
  .AddCustomMediator()
  .AddFeature<CreateProductCommand, CreateProductHandler>();
// highlight-end

var app = builder.Build();

// ... your endpoint mapping

app.Run();
```