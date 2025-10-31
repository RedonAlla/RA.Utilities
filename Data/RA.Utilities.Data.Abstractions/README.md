<p align="center">
  <img src="https://raw.githubusercontent.com/RedonAlla/RA.Utilities/main/Assets/Images/abstractions.svg" alt="RA.Utilities.Data.Abstractions Logo" width="128">
</p>

# RA.Utilities.Data.Abstractions

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Abstractions.svg)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.Abstractions.svg)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/)

`RA.Utilities.Data.Abstractions` provides a set of core interfaces for implementing common data access patterns, such as the Repository and Unit of Work. It solves the problem of tightly coupling your business logic to a specific data access technology (like Entity Framework Core).

By coding against these abstractions, you can create a clean, decoupled architecture where the data access layer can be swapped out with minimal impact on your application's core logic.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.Abstractions
```

### Prerequisites

This package contains only interfaces and is designed to be used with a concrete implementation. For example, you would create your own repository classes that implement these interfaces using an ORM like Entity Framework Core.

It is also recommended to use this package with `RA.Utilities.Data.Entities`, as the repository interfaces are constrained to `IEntity`.

## Usage

The package provides two main interfaces: `IRepository<TEntity, TKey>` and `IUnitOfWork`.

### `IRepository<TEntity, TKey>`

This generic interface defines standard CRUD (Create, Read, Update, Delete) operations for an entity.

```csharp
// An example of a concrete repository implementation
using RA.Utilities.Data.Abstractions;

public class ProductRepository : IRepository<Product, int>
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Products.FindAsync(id, cancellationToken);
    }

    public void Add(Product entity)
    {
        _context.Products.Add(entity);
    }

    // ... other method implementations
}
```

### `IUnitOfWork`

This interface represents a transaction that groups multiple repository operations. It ensures that a series of changes are either all committed or all rolled back, maintaining data integrity.

```csharp
// An example of a Unit of Work implementation with EF Core
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
```

### Injecting into a Service

You can then inject these abstractions into your business services, completely decoupling them from EF Core's `DbContext`.

```csharp
public class ProductService
{
    private readonly IRepository<Product, int> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IRepository<Product, int> productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the
[officially documentation](http://localhost:3000/RA.Utilities/nuget-packages/Data/Abstractions/).

## Feedback

If you have suggestions or find a bug, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities). Contributions are welcome!