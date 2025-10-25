<p align="center">
  <img src="../../Assets/Images/entity_framework_core.svg" alt="RA.Utilities.Data.EntityFramework Logo" width="128">
</p>

# RA.Utilities.Data.EntityFramework

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.EntityFramework.svg)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/)

Provides generic base classes for implementing the Repository and Unit of Work patterns with Entity Framework Core.
This package is the concrete implementation layer for the abstractions defined in `RA.Utilities.Data.Abstractions`.

## 🎯 Purpose

This library accelerates the setup of a data access layer by providing ready-to-use, generic base classes that handle common data operations. By using these implementations, you can:

- **Rapidly Implement Repositories**: Inherit from `RepositoryBase`, `ReadRepositoryBase`, or `WriteRepositoryBase` to get a full suite of data access methods out of the box.
- **Ensure Transactional Integrity**: Use the `UnitOfWork` implementation to manage database transactions and ensure that changes are saved atomically.
- **Promote Best Practices**: Encourages a clean, decoupled architecture by building on the abstractions from `RA.Utilities.Data.Abstractions`.

## ✨ Core Components

-   **`RepositoryBase<T>`**: A complete generic repository that implements both `IReadRepositoryBase<T>` and `IWriteRepositoryBase<T>`. It provides full CRUD functionality.
-   **`ReadRepositoryBase<T>`**: A read-only generic repository implementation, perfect for query-side operations in a CQS architecture. It uses `AsNoTracking()` by default for performance.
-   **`WriteRepositoryBase<T>`**: A write-only generic repository for command-side operations (Add, Update, Delete).

## 🛠️ Installation

Install the package via the .NET CLI:

```sh
dotnet add package RA.Utilities.Data.EntityFramework
```

Or through the NuGet Package Manager console:

```powershell
Install-Package RA.Utilities.Data.EntityFramework
```


## 🧩 Extensions
This package includes extension methods to simplify service registration for the generic repository patterns.

### DependencyInjectionExtensions 
Provides extension methods for setting up dependency injection for data-related services.

```
Namespace: RA.Utilities.Extensions
Source: Extensions/DependencyInjectionExtensions.cs
public static class DependencyInjectionExtensions
```
### Methods
#### AddRepositoryBase 
Adds the generic `IRepositoryBase<>` and its implementation `RepositoryBase<>` to the service collection as a scoped service. This allows you to inject `IRepositoryBase<T>` directly for any entity without creating a specific repository class.

Definition
```csharp
public static IServiceCollection AddRepositoryBase(this IServiceCollection services)
```

```csharp
// In Program.cs
builder.Services.AddRepositoryBase();
```

```csharp
// Now you can inject IRepositoryBase in your services
public class MyService(IRepositoryBase productRepository)
{
  // ... use productRepository 
}
```

#### AddReadRepositoryBase
Adds the generic `IReadRepositoryBase<>` and its implementation `ReadRepositoryBase<>` as a scoped service.
Use this if you only need read operations for certain entities, adhering to the Command Query Separation (CQS) principle.

**Definition**
```csharp
public static IServiceCollection AddReadRepositoryBase(this IServiceCollection services)
```

#### AddWriteRepositoryBase
Adds the generic `IWriteRepositoryBase<>` and its implementation `WriteRepositoryBase<>` as a scoped service.
Use this if you only need write operations for certain entities.

> [!NOTE]  
> The WriteRepositoryBase also contains the SaveChangesAsync method. In a Unit of Work pattern, saving changes is typically controlled by the UnitOfWork class, not individual repositories, to ensure transactional integrity.

**Definition**
```csharp
public static IServiceCollection AddWriteRepositoryBase(this IServiceCollection services)
```

---

## 🚀 Usage Guide

Here’s a step-by-step guide to setting up a data access layer using this package.

### 1. Define Your DbContext

Create your Entity Framework `DbContext` and make sure it implements `IDbContext` from `RA.Utilities.Data.Abstractions`.

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    // Other DbSets...
}
```

### 2. Implement Concrete Repositories

Create your own repository interfaces and classes. The interfaces should inherit from `IRepository<TEntity>` (from `RA.Utilities.Data.Abstractions`) and the classes should inherit from `Repository<TEntity>`.

```csharp
// In your Application/Domain layer (referencing RA.Utilities.Data.Abstractions)
public interface IProductRepository : IRepositoryBase<Product>
{
    Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count);
}

// In your Infrastructure/Data layer (referencing this package)
public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count)
    {
        // Custom data access logic
        return await _dbSet.OrderByDescending(p => p.UnitsSold).Take(count).ToListAsync();
    }
}
```

### 3. Register Services

In your `Program.cs` or `Startup.cs`, register the `DbContext`, the `UnitOfWork`, and your custom repositories.

```csharp
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.EntityFramework;

// ...

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

### 4. Use in Your Application

Inject `IUnitOfWork` into your services or controllers to access repositories and save changes.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetTopProducts()
    {
        var productRepository = _unitOfWork.GetRepository<IProductRepository>();
        var products = await productRepository.GetTopSellingProductsAsync(5);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        var productRepository = _unitOfWork.GetRepository<IProductRepository>();
        await productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(); // Commits the transaction
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
```

## 🔗 Dependencies

-   **RA.Utilities.Data.Abstractions**: Provides the core interfaces (`IRepository<T>`, `IUnitOfWork`, etc.) that this package implements.
-   **Microsoft.EntityFrameworkCore**: The underlying ORM used for the implementations.