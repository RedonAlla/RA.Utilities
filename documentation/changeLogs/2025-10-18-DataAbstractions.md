---
title: RA.Utilities.Data.Abstractions
authors: [RedonAlla]
---

## Version v10.0.0-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.0-rc.2)

This is the initial release of `RA.Utilities.Data.Abstractions`, a core library that provides essential abstractions for building a decoupled and testable data access layer. It introduces standard interfaces for the Repository and Unit of Work patterns.

<!-- truncate -->

### ✨ Key Features

*   **Generic Repository Interfaces**:
    *   `IRepositoryBase<T>`: A comprehensive interface for standard CRUD (Create, Read, Update, Delete) operations.
    *   `IReadRepositoryBase<T>` & `IWriteRepositoryBase<T>`: Segregated interfaces to better support the Command Query Separation (CQS) principle.

*   **`IUnitOfWork` Interface**:
    *   Provides a contract for managing transactions and persisting changes across multiple repositories.
    *   Includes a `SaveChangesAsync()` method to commit all changes in a single, atomic operation, ensuring data integrity.

*   **`IDbContext` Interface**:
    *   An abstraction for `DbContext` that allows your repository implementations to be decoupled from a concrete Entity Framework Core context, which greatly improves testability.

*   **Decoupling & Testability**: By depending on these abstractions instead of concrete data access technologies, your application and business logic remain persistence-ignorant, making them easier to mock and test.

### 🚀 Getting Started

1.  **Define Your Entities**: Create your entities using the base classes from `RA.Utilities.Data.Entities`.
    ```csharp
    public class Product : BaseEntity { /* ... */ }
    ```

2.  **Define Repository Interfaces**: In your application/domain layer, create specific repository interfaces.
    ```csharp
    public interface IProductRepository : IRepositoryBase<Product>
    {
        // Add custom query methods here
    }
    ```

3.  **Implement in Infrastructure**: Create concrete implementations of these interfaces in your infrastructure layer, typically using the `RA.Utilities.Data.EntityFramework` package.

4.  **Inject and Use**: Inject `IProductRepository` and `IUnitOfWork` into your services.