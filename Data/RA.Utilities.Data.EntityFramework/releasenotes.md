# Release Notes for RA.Utilities.Data.EntityFramework

## Version 1.0.0-preview.6.3

This is the initial preview release of the `RA.Utilities.Data.EntityFramework` package. It provides concrete implementations of the repository and unit of work patterns for Entity Framework Core, based on the abstractions from `RA.Utilities.Data.Abstractions`.

### ✨ New Features

*   **Initial Release**: Provides a set of generic base classes to quickly build a data access layer with EF Core.
*   **Generic Repository Implementations**:
    *   `RepositoryBase<T, TContext>`: A full implementation of `IRepository<T>` for complete CRUD functionality.
    *   `ReadRepositoryBase<T, TContext>`: A read-only repository that uses `AsNoTracking()` for efficient querying, ideal for CQS patterns.
    *   `WriteRepositoryBase<T, TContext>`: A write-only repository for command operations (Add, Update, Delete).
*   **Unit of Work Implementation**:
    *   `UnitOfWork<TContext>`: A concrete implementation of `IUnitOfWork` that manages the `DbContext` lifecycle and transaction commits via `SaveChangesAsync`.

### 💥 Breaking Changes

*   As this is the initial release, there are no breaking changes.

---