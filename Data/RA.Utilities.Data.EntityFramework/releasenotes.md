# Release Notes for RA.Utilities.Data.EntityFramework

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-27%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.0.2)

This release aligns the repository implementations with the `IEntity` marker interface introduced in `RA.Utilities.Data.Entities` 10.1.1, reworks the delete operations, and fixes timestamp handling in the save-changes interceptor.

### ✨ Enhancements
* ⁠Change the generic type constraint to the `IEntity` marker interface in all repository implementations. The `class` constraint is kept because EF Core's `DbSet<T>` requires reference types.

```csharp
+ public class RepositoryBase<T> : ReadRepositoryBase<T>, IRepositoryBase<T> where T : class, IEntity
- public class RepositoryBase<T> : ReadRepositoryBase<T>, IRepositoryBase<T> where T : CoreEntity

+ public class ReadRepositoryBase<T> : IReadRepositoryBase<T> where T : class, IEntity
- public class ReadRepositoryBase<T> : IReadRepositoryBase<T> where T : CoreEntity

+ public class WriteRepositoryBase<T> : IWriteRepositoryBase<T> where T : class, IEntity
- public class WriteRepositoryBase<T> : IWriteRepositoryBase<T> where T : CoreEntity
```

### 🔄 Behavior Changes
* `DeleteAsync` and `DeleteRangeAsync` now mark entities for deletion in the change tracker instead of deleting them immediately. The deletion is persisted when `SaveChangesAsync` (or the unit of work) is called, so it participates in the same transaction as any other pending changes. Previously, the delete statement executed immediately.
* `DeleteRangeAsync` accepts a `List<Guid>` of keys and returns the number of entities that were found and marked for deletion.

### 🐛 Bug Fixes
* `BaseEntitySaveChangesInterceptor` now detects timestamp properties through EF Core model metadata instead of a typed `Entries<BaseEntity>()` scan. As a result, the `LastModifiedAt` property of `WriteEntity<TKey>` descendants is now updated correctly (previously it was never set).

### 📦 Dependencies
* Requires `RA.Utilities.Data.Abstractions` **10.0.3** and `RA.Utilities.Data.Entities` **10.1.1** or later.

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-14%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.0.1)

### ✨ No Breaking Changes
Change generic type constraint from `BaseEntity` in to `CoreEntity` in repository interfaces.

```csharp
+ public interface IReadRepositoryBase<T> where T : notnull, CoreEntity
- public interface IReadRepositoryBase<T> where T : notnull, BaseEntity
```

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.0.0)

Updated package version `10.0.9-rc` (release candidate) to `10.0.0`, indicating a transition to a stable release.
This change signifies that the project is no longer in the release candidate phase and is considered ready for production use, reflecting confidence in its stability and completeness.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.0.0-rc.2)

This release of `RA.Utilities.Data.EntityFramework` provides concrete implementations of the Repository and Unit of Work patterns for Entity Framework Core. It serves as the persistence layer for the abstractions defined in `RA.Utilities.Data.Abstractions`.

### ✨ Key Features

*   **Generic Repository Implementations**:
    *   `RepositoryBase<T>`: A full implementation of `IRepositoryBase<T>` for complete CRUD functionality.
    *   `ReadRepositoryBase<T>`: A read-only repository that uses `AsNoTracking()` by default for efficient querying, ideal for CQS patterns.
    *   `WriteRepositoryBase<T>`: A write-only repository for command operations (Add, Update, Delete).

*   **Automatic Timestamping**:
    *   `BaseEntitySaveChangesInterceptor`: An Entity Framework Core interceptor that automatically populates `CreatedAt` and `LastModifiedAt` properties on entities inheriting from `BaseEntity` before they are saved. This ensures consistent and accurate auditing without manual intervention.

*   **Generic Unit of Work Implementation**:
    *   `UnitOfWork<TContext>`: A generic implementation of `IUnitOfWork` that manages the `DbContext` lifecycle and ensures transactional integrity by saving all changes atomically.

*   **Dependency Injection Extensions**:
    *   Includes `AddRepositoryBase()`, `AddReadRepositoryBase()`, and `AddWriteRepositoryBase()` extension methods to simplify the registration of generic repositories in your application's DI container.

### 🚀 Getting Started

1.  **Define Your DbContext**: Create your `ApplicationDbContext` inheriting from `DbContext`.
    ```csharp
    public class ApplicationDbContext : DbContext, IDbContext
    {
        // ... DbSets
    }
    ```

2.  **Register Services**: In `Program.cs`, register your `DbContext`, the `UnitOfWork`, and your repositories.
    ```csharp
    // Register DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(...);

    // Register UnitOfWork and generic repositories
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
    builder.Services.AddRepositoryBase(); // Registers IRepositoryBase<>
    ```

3.  **Use in Your Application**: Inject `IUnitOfWork` or `IRepositoryBase<T>` into your services to interact with the database.
    ```csharp
    public class ProductService(IRepositoryBase<Product> productRepo, IUnitOfWork uow)
    {
        // ... use repository methods and uow.SaveChangesAsync()
    }
    ```