# RA.Utilities.Data.Abstractions

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Abstractions.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.Abstractions.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/Abstractions/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)


`RA.Utilities.Data.Abstractions` provides a collection of essential interfaces for building a decoupled and testable data access layer. It establishes contracts for the **Repository** and **Unit of Work** patterns, which are fundamental to Clean Architecture.

By coding against these abstractions, you can create a clean, decoupled architecture where the data access layer can be swapped out with minimal impact on your application's core logic.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.Abstractions
```

## ✨ Core Abstractions
This package provides a set of interfaces to enforce a clean, CQS-friendly data access strategy.

### Repository Interfaces
The repository pattern is split into read and write interfaces to support Command Query Separation (CQS).

* **`IReadRepositoryBase<T>`**: Defines read-only operations like `GetByIdAsync` and `ListAsync`.
Implementations of this interface are optimized for querying and should not modify data.

* **`IWriteRepositoryBase<T>`**: Defines write-only operations like `AddAsync`, `UpdateAsync`, and `DeleteAsync`. 

* **`IRepositoryBase<T>`**: A convenience interface that inherits from both `IReadRepositoryBase<T>` and `IWriteRepositoryBase<T>`, providing a full suite of CRUD operations.

### IDbContext and IUnitOfWork
* **`IDbContext`**: A marker interface that your `DbContext` should implement.
This allows repository implementations to depend on an abstraction rather than a concrete `DbContext`, which is crucial for unit testing.
* **`IUnitOfWork`**: Defines a contract for managing transactions.
Its `SaveChangesAsync()` method ensures that all changes made across multiple repositories are saved as a single, atomic operation.

## 🚀 Usage

These abstractions are designed to be implemented in your Infrastructure layer and consumed by your Application layer.

### 1. Define Your Repository Interface
In your Application layer, define an interface for your entity that inherits from the base abstractions.

```csharp
// An example of a concrete repository implementation
using RA.Utilities.Data.Abstractions;
using YourApp.Domain.Entities;

public class ProductRepository : IRepositoryBase<Product>
{
    // You can add custom, entity-specific query methods here
    Task<Product?> GetProductBySkuAsync(string sku);
}
```

### 2. Implement the Concrete Repository
In your Infrastructure layer, implement the interface. You can inherit from `RepositoryBase<T>` (provided by `RA.Utilities.Data.EntityFramework`) to get the standard implementations for free.

```csharp
using RA.Utilities.Data.EntityFramework;
using YourApp.Domain.Entities;
using YourApp.Persistence;
public class ProductRepository : IRepository<Product, int>
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetProductBySkuAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Product>().FirstOrDefaultAsync(p => p.Sku == sku);
    }

    public void Add(Product entity)
    {
        _context.Products.Add(entity);
    }

    // ... other method implementations
}
```

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the
[officially documentation](http://localhost:3000/RA.Utilities/nuget-packages/Data/Abstractions/).

## Feedback

If you have suggestions or find a bug, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities). Contributions are welcome!