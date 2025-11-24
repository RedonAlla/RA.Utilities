# Release Notes for RA.Utilities.Data.EntityFramework

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