# Release Notes for RA.Utilities.Data.Abstractions

## Version 10.0.3
![Date Badge](https://img.shields.io/badge/Publish-27%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.3)

This release decouples the repository contracts from the concrete entity base classes.
All repository interfaces now constrain their entity type to the `IEntity` marker interface (introduced in `RA.Utilities.Data.Entities` 10.1.1) instead of `CoreEntity`,
so custom entities outside the `CoreEntity<TKey>` hierarchy can now flow through the repository abstractions.

### ✨ Enhancements
* ⁠Change the generic type constraint from `CoreEntity` to the `IEntity` marker interface in all repository interfaces.

```csharp
+ public interface IReadRepositoryBase<T> : IRepository where T : notnull, IEntity
- public interface IReadRepositoryBase<T> : IRepository where T : notnull, CoreEntity

+ public interface IWriteRepositoryBase<T> where T : IEntity
- public interface IWriteRepositoryBase<T> where T : CoreEntity

+ public interface IRepositoryBase<T> : IReadRepositoryBase<T>, IWriteRepositoryBase<T> where T : IEntity
- public interface IRepositoryBase<T> : IReadRepositoryBase<T>, IWriteRepositoryBase<T> where T : CoreEntity
```

### 📦 Dependencies
* Requires `RA.Utilities.Data.Entities` **10.1.1** or later (for the `IEntity` marker interface).

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-11%20January%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.2)

### ✨ Enhancements
* ⁠Added a new interface `IRepository`⁠, serving as a marker interface for all repository types in the application.

```csharp
public interface IRepository;
```

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-14%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.1)

This release introduces a significant architectural refactoring of the repository interfaces to better align with Command Query Separation (CQS) principles and simplify their usage.

### ✨ No Breaking Changes
Change generic type constraint from `BaseEntity` in to `CoreEntity` in repository interfaces.

```csharp
+ public interface IReadRepositoryBase<T> where T : notnull, CoreEntity
- public interface IReadRepositoryBase<T> where T : notnull, BaseEntity
```

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.0)

Updates the version from `10.0.0-rc.2` (release candidate) to `10.0.0`, signifying the transition to a stable release.
This change indicates that the library is now considered complete and ready for general use, reflecting confidence in its stability and functionality.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Abstractions/10.0.0-rc.2)

This release of `RA.Utilities.Data.Abstractions` provides a collection of essential data access abstractions, including the Repository and Unit of Work patterns. This library is the foundation for creating a decoupled and testable data access layer in .NET applications.

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