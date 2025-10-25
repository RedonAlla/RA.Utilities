---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

The `ReadRepositoryBase<T>` class is a concrete implementation of a generic, **read-only repository** using ***Entity Framework Core***.
Its primary purpose is to provide a standardized and efficient way to query data from a database without allowing any modifications.

This class is a direct application of the **Command Query Separation (CQS)** principle, which is a core concept in the `RA.Utilities` ecosystem.
It ensures that methods are either "Queries" (retrieving data) or "Commands" (changing data), but not both.

## 🔑 Key Purposes and Benefits
#### 1. Enforces Read-Only Access:
By inheriting from or using `ReadRepositoryBase`, you create a data access component that is guaranteed not to have methods for adding, updating, or deleting data.
This is perfect for the "Query" side of a CQRS architecture, reporting services, or any part of your application that should only display information.

#### 2. Performance Optimization:
The documentation highlights a crucial implementation detail: `ReadRepositoryBase` uses ***Entity Framework Core***'s `.AsNoTracking()` method by default for all its queries.
This tells EF Core not to track changes on the retrieved entities, which significantly reduces memory overhead and improves query performance, as there's no need to manage the state of objects that will never be updated.

#### 3. Accelerates Development:
It provides out-of-the-box implementations for common query operations defined in the [`IReadRepositoryBase<T>`](../Abstractions/IReadRepositoryBase.md) interface, such as:

  * `GetByIdAsync<TId>`: To fetch a single entity by its primary key.
  * `ListAsync`: A flexible method to get a list of entities with support for filtering, sorting, and pagination.

This means you don't have to write this boilerplate query logic yourself for every entity.

#### 4. Promotes Clean Architecture:
It allows your application services to depend on a specific, read-only contract ([`IReadRepositoryBase<T>`](../Abstractions/IReadRepositoryBase.md)).
This makes the intent of your code clearer and prevents developers from accidentally modifying data in a part of the application that is only supposed to read it.

In short, `ReadRepositoryBase` is a specialized tool for building the query side of your data access layer. It's designed to be fast, safe, and to help you maintain a clean, separated architecture.

## 🛠️ Example
Here is an example of how you might register and use it based on the provided documentation:

```csharp
// In Program.cs, register the generic read-only repository
builder.Services.AddReadRepositoryBase();
```

```csharp showLineNumbers
// In a service that only needs to read data
public class ProductReportService
{
    // Inject the read-only repository for Products
    private readonly IReadRepositoryBase<Product> _productReadRepository;

    public ProductReportService(IReadRepositoryBase<Product> productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsReport()
    {
        // This repository only has query methods, preventing any accidental writes.
        // The query will also be more performant due to AsNoTracking().
        var activeProducts = await _productReadRepository.ListAsync(
            filter: p => p.IsActive,
            orderBy: q => q.OrderBy(p => p.Name)
        );

        return activeProducts;
    }
}
```