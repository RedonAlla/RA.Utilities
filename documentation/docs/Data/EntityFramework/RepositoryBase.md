---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

The `RepositoryBase<T>` class is a concrete implementation of a generic,
**full-featured repository** that provides both read and write (full CRUD) operations for a given entity `T`.

Its primary purpose is to serve as a convenient, all-in-one data access component for services that need to perform both querying and data modification, implementing the composite [`IRepositoryBase<T>`](../Abstractions/IRepositoryBase.md) interface.

## ⚙️ How It Works
The class cleverly combines the specialized read-only and write-only base classes to achieve its functionality:

#### 1. Inheritance for Read Operations:
It inherits from [`ReadRepositoryBase<T>`](./ReadRepositoryBase.md). This means it directly gets all the performance-optimized, read-only methods like GetByIdAsync and ListAsync (which use .AsNoTracking() by default).

#### 2. Composition for Write Operations:
It composes a private instance of [`WriteRepositoryBase<T>`](WriteRepositoryBase.md).
It then delegates all the write method calls (`AddAsync`, `UpdateAsync`, `DeleteAsync`, etc.) to this internal `_writeRepository` instance.

This design pattern is known as **Composition over Inheritance**.
Instead of creating a complex inheritance chain, it reuses functionality by holding an instance of another class.

```csharp showLineNumbers
// 1. Inherits from ReadRepositoryBase<T>
public class RepositoryBase<T> : ReadRepositoryBase<T>, IRepositoryBase<T>
    where T : BaseEntity
{
    // 2. Composes WriteRepositoryBase<T>
    private readonly WriteRepositoryBase<T> _writeRepository;

    public RepositoryBase(DbContext dbContext) : base(dbContext)
    {
        // Initializes the composed write repository
        _writeRepository = new WriteRepositoryBase<T>(dbContext);
    }

    // ...

    // 3. Delegates write calls to the composed instance
    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        => _writeRepository.AddAsync(entity, cancellationToken);

    // ... other delegated write methods
}
```

## 🛠️ When to Use It
You would use `RepositoryBase<T>` (or inject [`IRepositoryBase<T>`](../Abstractions/IRepositoryBase.md)) in services where the separation between commands and queries is not strictly required.
It's a practical and common choice for many application services that handle standard business logic involving both fetching and saving data for an entity.
