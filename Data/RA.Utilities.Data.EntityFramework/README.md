# RA.Utilities.Data.EntityFramework

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.EntityFramework?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.EntityFramework.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/EntityFramework/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

This package provides concrete implementations of the repository and unit of work patterns for Entity Framework Core, based on the abstractions from `RA.Utilities.Data.Abstractions`. It's designed to accelerate the setup of a data access layer in a clean, testable, and maintainable way.

## ✨ Key Features
* **Generic Repository Implementations**: Provides ready-to-use base classes for repository patterns, saving you from writing boilerplate CRUD (Create, Read, Update, Delete) code.
* **Command Query Separation (CQS)**: Offers distinct `ReadRepositoryBase<T>` and `WriteRepositoryBase<T>` classes to help you build a clean architecture where read and write operations are separated.
* **Performance-Optimized Queries**: The `ReadRepositoryBase<T>` uses `AsNoTracking()` by default for more efficient data retrieval.
* **Automatic Timestamping**: Includes a `BaseEntitySaveChangesInterceptor` that automatically sets `CreatedAt` and `LastModifiedAt` properties on your entities when they are saved.
* **Simplified Dependency Injection**: Provides extension methods to register your repositories with a single line of code in `Program.cs`.

## Installation
You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.EntityFramework
```

```powershell
Install-Package RA.Utilities.Data.EntityFramework
```

## Core Components

### Repository Implementations
This package provides concrete implementations for the interfaces defined in `RA.Utilities.Data.Abstractions`.

* **`RepositoryBase<T>`**: A full-featured generic repository that implements `IRepositoryBase<T>` for complete CRUD functionality. It's a convenient all-in-one solution for services that need to both query and modify data.
* **`ReadRepositoryBase<T>`**: A read-only repository implementing `IReadRepositoryBase<T>`. It is optimized for performance by using `AsNoTracking()` on queries, making it ideal for the "Query" side of a CQRS architecture.
* **`WriteRepositoryBase<T>`**: A write-only repository implementing `IWriteRepositoryBase<T>`. It is designed for "Command" operations like adding, updating, and deleting entities.

### Interceptors
* **`BaseEntitySaveChangesInterceptor`**: An Entity Framework Core interceptor that automatically populates timestamp properties (`CreatedAt`, `LastModifiedAt`) on entities inheriting from `BaseEntity` before changes are saved to the database. This ensures consistent and accurate auditing without manual intervention.

### Dependency Injection Extensions

The `DependencyInjectionExtensions` class simplifies the registration of the generic repositories in your application's DI container.

* `AddRepositoryBase()`: Registers the full `IRepositoryBase<> `implementation.
* `AddReadRepositoryBase()`: Registers the read-only `IReadRepositoryBase<>` implementation.
* `AddWriteRepositoryBase()`: Registers the write-only `IWriteRepositoryBase<>` implementation.

## Usage Example
Here is how you can configure and use the components from this package in an ASP.NET Core application.

### 1. Define Your DbContext
First, ensure your `DbContext` implements the `IDbContext` interface from `RA.Utilities.Data.Abstractions`.

```csharp
public class ApplicationDbContext : DbContext, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
```

### 2. Register Services in `Program.cs`
In your `Program.cs`, register your `DbContext`, the interceptor, and the desired repository implementations.

```csharp
using RA.Utilities.Data.EntityFramework.Extensions;
using RA.Utilities.Data.EntityFramework.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// 1. Register the SaveChangesInterceptor
builder.Services.AddScoped<BaseEntitySaveChangesInterceptor>();

// 2. Register the DbContext and add the interceptor
builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(provider.GetRequiredService<BaseEntitySaveChangesInterceptor>()));

// 3. Register the generic repositories
builder.Services.AddRepositoryBase(); // For IRepositoryBase<>
builder.Services.AddReadRepositoryBase(); // For IReadRepositoryBase<>
builder.Services.AddWriteRepositoryBase(); // For IWriteRepositoryBase<>

// Register your custom repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();
```

### 3. Implement a Custom Repository (Optional)
You can extend the generic repositories to add custom data access methods.

```csharp
public interface IProductRepository : IRepositoryBase<Product>
{
    Task<Product?> GetProductByNameAsync(string name);
}

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product?> GetProductByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }
}
```

### 4. Use in a Service
Finally, inject the repository interfaces into your services to interact with the database.

```csharp
public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductDetailsAsync(Guid id)
    {
        // Use a method from the base repository
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task CreateProductAsync(string name, decimal price)
    {
        var newProduct = new Product { Name = name, Price = price };
        
        // The BaseEntitySaveChangesInterceptor will automatically set CreatedAt
        await _productRepository.AddAsync(newProduct);
    }
}
```