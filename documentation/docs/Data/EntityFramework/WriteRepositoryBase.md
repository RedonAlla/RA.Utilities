---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

The `WriteRepositoryBase<T>` class is a concrete implementation of a generic, **write-only repository** using ***Entity Framework Core***.
Its primary purpose is to provide a standardized way to modify data in a database (`Create`, `Update`, `Delete`) while preventing any read operations.

This class is a key component for implementing the **Command Query Separation (CQS)** principle.
It represents the "Command" side, where the sole responsibility is to change the state of the system.

## 🔑 Key Purposes and Benefits
#### 1. Enforces Write-Only Access:
By using or inheriting from `WriteRepositoryBase`, you create a data access component that is guaranteed to only have methods for mutating data, such as `AddAsync`, `UpdateAsync`, and `DeleteAsync`.
This is ideal for the "Command" side of a CQRS architecture or any service whose only job is to process state changes.

#### 2. Promotes Clean Architecture:
It allows your application services to depend on a specific, write-only contract ([`IWriteRepositoryBase<T>`](../Abstractions/IWriteRepositoryBase.md)).
This makes the intent of your code exceptionally clear and prevents developers from accidentally querying data in a part of the application that is only supposed to modify it.

#### 3. Accelerates Development:
It provides ready-to-use implementations for all the common data modification operations defined in the [`IWriteRepositoryBase<T>`](../Abstractions/IWriteRepositoryBase.md) interface.
This saves you from writing this repetitive boilerplate code for every entity in your system.


In short, `WriteRepositoryBase` is a specialized tool for building the command side of your data access layer.
It's designed to be safe, explicit, and to help you maintain a clean, separated architecture.

## 🧐 Code Example
Here is how you might use it in a service that handles commands, based on the patterns in the documentation:

```csharp showLineNumbers
// In a service that only needs to write data
public class ProductCommandService
{
    // Inject the write-only repository for Products
    private readonly IWriteRepositoryBase<Product> _productWriteRepository;

    public ProductCommandService(IWriteRepositoryBase<Product> productWriteRepository, IUnitOfWork unitOfWork)
    {
        _productWriteRepository = productWriteRepository;
    }

    public async Task CreateNewProduct(string name, decimal price)
    {
        var newProduct = new Product { Name = name, Price = price };

        // This repository only has write methods, preventing any accidental reads.
        await _productWriteRepository.AddAsync(newProduct);

        // The UnitOfWork is responsible for saving the change to ensure transactional integrity.
        await _productWriteRepository.SaveChangesAsync();
    }
}
```

This example demonstrates how the `ProductCommandService` is restricted to only writing `Product` data.