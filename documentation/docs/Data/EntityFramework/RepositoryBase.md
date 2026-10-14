---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

The `RepositoryBase<T, TKey>` class is a concrete implementation of a generic,
**full-featured repository** that provides both read and write (full CRUD) operations for a given entity `T` with primary key type `TKey`.

Its primary purpose is to serve as a convenient, all-in-one data access component for services that need to perform both querying and data modification, implementing the composite [`IRepositoryBase<T, TKey>`](../Abstractions/IRepositoryBase.md) interface.

## ⚙️ How It Works
The class cleverly combines the specialized read-only and write-only base classes to achieve its functionality:

#### 1. Inheritance for Read Operations:
It inherits from [`ReadRepositoryBase<T, TKey>`](./ReadRepositoryBase.md). This means it directly gets all the performance-optimized, read-only methods like GetByIdAsync and ListAsync (which use .AsNoTracking() by default).

#### 2. Composition for Write Operations:
It composes a private instance of [`WriteRepositoryBase<T, TKey>`](WriteRepositoryBase.md).
It then delegates all the write method calls (`AddAsync`, `UpdateAsync`, `DeleteAsync`, etc.) to this internal `_writeRepository` instance.

This design pattern is known as **Composition over Inheritance**.
Instead of creating a complex inheritance chain, it reuses functionality by holding an instance of another class.

```csharp showLineNumbers
// 1. Inherits from ReadRepositoryBase<T, TKey>
public class RepositoryBase<T, TKey> : ReadRepositoryBase<T, TKey>, IRepositoryBase<T, TKey>
    where T : CoreEntity<TKey>
    where TKey : notnull
{
    // 2. Composes WriteRepositoryBase<T, TKey>
    private readonly WriteRepositoryBase<T, TKey> _writeRepository;

    public RepositoryBase(DbContext dbContext) : base(dbContext)
    {
        // Initializes the composed write repository
        _writeRepository = new WriteRepositoryBase<T, TKey>(dbContext);
    }

    // ...

    // 3. Delegates write calls to the composed instance
    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        => _writeRepository.AddAsync(entity, cancellationToken);

    // ... other delegated write methods
}
```

The entity type parameter is constrained to [`CoreEntity<TKey>`](../Entities/CoreEntity.md), so any class from the `RA.Utilities.Data.Entities` hierarchy (including `BaseEntity<TKey>`, `WriteEntity<TKey>`, and their derivations) can be used; the key type parameter `TKey` can be any non-nullable type (`Guid`, `int`, `long`, `string`, …).

## 🛠️ When to Use It
You would use `RepositoryBase<T, TKey>` (or inject [`IRepositoryBase<T, TKey>`](../Abstractions/IRepositoryBase.md)) in services where the separation between commands and queries is not strictly required.
It's a practical and common choice for many application services that handle standard business logic involving both fetching and saving data for an entity.
