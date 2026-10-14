# Release Notes for RA.Utilities.Data.EntityFramework

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-11%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.1.0)

This release adds persistence support for the value objects of `RA.Utilities.Core.ValueObjects` — EF Core value converters and a `HasMoney` convention that stores a `Money` in two database columns — renames `DependencyInjectionExtensions` to `ServiceCollectionExtensions`, and refactors the extension classes to C# 14 extension members.

### ✨ New Features

* **Added `MoneyConventionExtensions.HasMoney`**: Maps a `Money` property to two separate database columns — `Amount` in a decimal column and `Currency` in a string column (through `CurrencyConverter`) — instead of storing the whole value in a single column.
    * EF Core value converters map a property to a single column, so `HasMoney` maps the property as an EF Core complex property instead. Columns are named after the property, e.g. `Balance_Amount` and `Balance_Currency`; the `Amount` column uses the provider default for `decimal` (`decimal(18,2)` on SQL Server, `TEXT` on SQLite).
    * Supports required (`Money`), optional (`Money?`, stored as `NULL` in both columns) and multiple `Money` properties on the same entity. Because `Money` is immutable, updates persist by replacing the instance on a tracked entity.
    * For single-column storage, `MoneyConverter` (single JSON string column) is the alternative.
* **Added value converters** (namespace `RA.Utilities.Data.EntityFramework.Conventions`): `CurrencyConverter`, `EmailConverter` and `SsnConverter` map the string-based value objects to and from their normalized `Value` string; `MoneyConverter` serializes the whole `Money` as JSON into a single string column.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Wallet>().HasMoney(w => w.Balance);
}
```

### ⚠️ Breaking Changes

* **`DependencyInjectionExtensions` renamed to `ServiceCollectionExtensions`**: The class providing `AddRepositoryBase()`, `AddReadRepositoryBase()` and `AddWriteRepositoryBase()` now follows the ASP.NET Core naming convention. The namespace `RA.Utilities.Data.EntityFramework.Extensions` and all method signatures are unchanged, so extension-method call sites such as `builder.Services.AddRepositoryBase()` keep compiling; only code that references the class by name — a `typeof(...)` expression, a static call or a reflection string — needs updating.

```csharp
- var registrationType = typeof(DependencyInjectionExtensions);
+ var registrationType = typeof(ServiceCollectionExtensions);
```

### 📝 Improvements

* **C# 14 extension members**: `ServiceCollectionExtensions` and the `HasMoney` convention in `MoneyConventionExtensions` now declare their methods inside `extension(...)` blocks. The generated members keep the same names, signatures and namespaces, so existing call sites are unaffected.

### 📦 Dependencies
* New dependency on `RA.Utilities.Core.ValueObjects` **10.1.0**. `RA.Utilities.Data.Abstractions` and `Microsoft.EntityFrameworkCore` are unchanged.

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-27%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.EntityFramework/10.0.2)

This release makes all repository implementations generic over the primary key type (`TKey`), reworks the delete operations, and fixes timestamp handling in the save-changes interceptor. `RA.Utilities.Data.Entities` 10.1.1 introduces the `IEntity` marker interface, implemented by `CoreEntity<TKey>`.

### ⚠️ Breaking Changes
* **Repository classes and interfaces are now generic over the primary key type (`TKey`)**: `RepositoryBase<T>`, `ReadRepositoryBase<T>`, `WriteRepositoryBase<T>` and their interfaces became `<T, TKey>`, and `GetByIdAsync` / `DeleteAsync` / `DeleteRangeAsync` now take a `TKey` instead of a `Guid`. The entity type parameter is constrained to `CoreEntity<TKey>` (`where T : CoreEntity<TKey> where TKey : notnull`), so numeric and string keys are supported alongside `Guid`; the `IEntity` marker interface added in `RA.Utilities.Data.Entities` 10.1.1 is implemented by `CoreEntity<TKey>`.

```csharp
+ public interface IReadRepositoryBase<T, TKey> : IRepository where T : CoreEntity<TKey> where TKey : notnull
- public interface IReadRepositoryBase<T> : IRepository where T : notnull, CoreEntity

+ Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
- Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
```

### 🔄 Behavior Changes
* `DeleteAsync` and `DeleteRangeAsync` now mark entities for deletion in the change tracker instead of deleting them immediately. The deletion is persisted when `SaveChangesAsync` (or the unit of work) is called, so it participates in the same transaction as any other pending changes. Previously, the delete statement executed immediately.
* `DeleteRangeAsync` accepts an `IEnumerable<TKey>` of keys and returns the number of entities that were found and marked for deletion.

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