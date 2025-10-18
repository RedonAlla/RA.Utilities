# RA.Utilities

## The Big Picture
Your goal is to have clean, maintainable, and robust code. The `RA.Utilities` ecosystem provides the building blocks for each layer of this architecture.

Here’s how the packages fit together:

| Layer/Concern	| RA.Utilities Package(s)	| Purpose |
| ------------- | ----------------------- | ------- |
| **1. Configuration & Startup** | `RA.Utilities.Api`, `RA.Utilities.OpenApi`, `RA.Utilities.Authentication.JwtBearer` |	Sets up the web host, middleware, DI container, | authentication, and OpenAPI documentation. |
| **2. API Endpoints** |	`RA.Utilities.Api` |	Defines the API routes and handles incoming HTTP requests. |
| **3. Application Logic (CQRS)** |	`RA.Utilities.Feature`, `RA.Utilities.Core` |	Orchestrates the business logic for a single feature using commands, queries, and handlers.|
| **4. Data Access** |	`RA.Utilities.Data.Abstractions`, `RA.Utilities.Data.EntityFramework` |	Provides abstractions and implementations for talking to the database. |
| **Cross-Cutting** |	`RA.Utilities.Core.Constants`, `RA.Utilities.Core` | (Exceptions)	Provides shared constants and a standardized way to handle errors. |

---

## A Walkthrough: Creating a New Product

Let's trace a `POST /api/products` request through an application built with these packages.

### Step 1: Project Setup (`Program.cs`)
Your `Program.cs` is the composition root where everything is wired together.

```csharp
// --- Program.cs ---
using RA.Utilities.Api.Extensions;
using RA.Utilities.Authentication.JwtBearer.Extensions;
using RA.Utilities.Feature.Behaviors;
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURE SERVICES

// Add JWT Authentication from RA.Utilities.Authentication.JwtBearer
// Reads settings from appsettings.json
builder.Services.AddJwtBearerAuthentication(builder.Configuration);

// Add OpenAPI (Swagger) services and transformers from RA.Utilities.OpenApi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer() // Adds Info, Bearer, and Headers transformers
    .AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>(); // Adds standard error responses

// Add CQRS and Validation pipeline from RA.Utilities.Feature
services.AddCustomMediator();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Add Data Access Layer (using RA.Utilities.Data abstractions and EF implementation)
// (Assuming you have an AddPersistence extension method for this)
builder.Services.AddPersistence(builder.Configuration);


// 2. CONFIGURE MIDDLEWARE PIPELINE
var app = builder.Build();

// Add Global Exception Handling from RA.Utilities.Api
// This catches exceptions like NotFoundException and returns a standard JSON response.
app.UseRaExceptionHandling();

if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoints from RA.Utilities.OpenApi
    app.MapOpenApi();
}

// Add Authentication & Authorization middleware
app.UseAuth(); // Convenience method from RA.Utilities.Authentication.JwtBearer

// Discover and map all IEndpoint implementations from RA.Utilities.Api
app.MapEndpoints();

app.Run();
```

### Step 2: The Feature Slice (`CreateProduct` Command & Handler)
This is the core business logic, organized as a vertical slice using `RA.Utilities.Feature`.

```csharp
// --- Features/Products/CreateProduct.cs ---

// The Command (Request)
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<int>>;

// The Validator (using FluentValidation)
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// The Handler (using RA.Utilities.Feature)
public class CreateProductHandler : BaseHandler<CreateProductCommand, Result<int>>
{
    private readonly IProductRepository _productRepository; // From RA.Utilities.Data.Abstractions
    private readonly IUnitOfWork _unitOfWork;             // From RA.Utilities.Data.Abstractions

    public CreateProductHandler(IProductRepository repo, IUnitOfWork uow, ILogger<CreateProductHandler> logger) 
        : base(logger)
    {
        _productRepository = repo;
        _unitOfWork = uow;
    }

    public override async Task<Result<int>> HandleAsync(CreateProductCommand command, CancellationToken ct)
    {
        var newProduct = new Product { Name = command.Name, Price = command.Price };
        
        await _productRepository.AddAsync(newProduct, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Return a success Result from RA.Utilities.Core
        return newProduct.Id;
    }
}
```

#### How the packages interact here:
* `RA.Utilities.Feature`: The `ValidationBehavior` automatically runs `CreateProductCommandValidator`. The BaseHandler provides logging and exception safety.
* `RA.Utilities.Data.Abstractions`: The handler depends on `IProductRepository` and not a concrete `DbContext`.
* `RA.Utilities.Core`: The handler returns a `Result<int>` to clearly communicate success or failure without throwing exceptions for business rules.

### Step 3: The API Endpoint (`ProductEndpoints.cs`)
This class, using `RA.Utilities.Api,` defines the HTTP route and connects it to the CQRS handler.

```csharp
// --- Endpoints/ProductEndpoints.cs ---
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Core.Constants; // Using constants for clarity

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapPost("/", async (CreateProductCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            // Use the Match extension from RA.Utilities.Core to handle the Result
            return result.Match(
                // On success, return 201 Created
                productId => Results.Created($"/api/products/{productId}", new { id = productId }),
                
                // On failure, let the middleware handle it.
                // A ValidationException from the pipeline will become a 400.
                // A ConflictException thrown in the handler would become a 409.
                exception => throw exception
            );
        })
        .Produces(HttpStatusCodes.Created) // Using RA.Utilities.Core.Constants
        .Produces<ValidationProblemDetails>(HttpStatusCodes.BadRequest)
        .Produces<ConflictResponse>(HttpStatusCodes.Conflict); // Model from RA.Utilities.Api
    }
}
```

#### How the packages interact here:
* `RA.Utilities.Api`: The `IEndpoint` interface and `MapEndpoints()` call in `Program.cs` register this route. The `ConflictResponse` type is used for OpenAPI documentation.
* `RA.Utilities.Core`: The endpoint uses `result.Match()` to handle the success case. For failures, it re-throws the exception, which is caught by the `UseRaExceptionHandling()` middleware.
* `RA.Utilities.Core.Constants`: `HttpStatusCodes.Created` is used instead of the magic number `201`.
* `RA.Utilities.OpenApi`: The `Produces` attributes, combined with the `ResponsesDocumentTransformer`, ensure the Swagger UI accurately documents the possible success and error responses.

## Summary of the Flow
1. A `POST` request hits `/api/products`.
2. The `ProductEndpoints` class handles it, creates a `CreateProductCommand`, and sends it via `IMediatr`.
3. The `ValidationBehavior` from `RA.Utilities.Feature` intercepts the command and runs the validator. If it fails, a `ValidationException` is returned immediately, which the endpoint middleware turns into a **400 Bad Request**.
4. If valid, the `CreateProductHandler` executes. It uses the `IProductRepository` (implemented by `RA.Utilities.Data.EntityFramework`) to add the product and IUnitOfWork to save it.
5. The handler returns a `Result<int>`.
6. The endpoint receives the `Result`. On success, it returns a **201 Created**. If the handler had returned a failure (e.g., `new ConflictException()`), the endpoint would throw it, and the `UseRaExceptionHandling()` middleware would turn it into a **409 Conflict response**.
7. All of this is automatically and correctly documented in Swagger, thanks to `RA.Utilities.OpenApi`.

Together, these packages provide a powerful, cohesive, and configuration-driven framework for building clean, scalable, and maintainable ASP.NET Core APIs.