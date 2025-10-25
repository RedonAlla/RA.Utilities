---
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Data.Abstractions
```

The `IRepositoryBase<T>` interface serves as a unified, all-in-one contract for a generic repository.
Its primary purpose is to combine both read and write operations into a single, convenient interface for entities that require full CRUD (`Create`, `Read`, `Update`, `Delete`) functionality.

It achieves this by inheriting from two more specialized interfaces:

1. **`IReadRepositoryBase<T>`**: This interface defines all the methods for querying data (e.g., `GetByIdAsync`, `ListAsync`).
2. **`IWriteRepositoryBase<T>`**: This interface defines all the methods for mutating data (e.g., `AddAsync`, `UpdateAsync`, `DeleteAsync`).

By inheriting from both, `IRepositoryBase<T>` provides a complete set of standard data access methods without defining any of its own.

```csharp showLineNumbers
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Defines a base interface for both read and write repository operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T> : IReadRepositoryBase<T>, IWriteRepositoryBase<T> where T : BaseEntity
{

}
```

## Why is this useful?
This design supports the **Command Query Separation (CQS)** principle by providing separate `IReadRepositoryBase` and `IWriteRepositoryBase` interfaces.
However, in many common scenarios, a repository needs to handle both reading and writing.
`IRepositoryBase<T>` acts as a convenient composite interface for these cases.

When you create a specific repository interface, like `IProductRepository`, you can inherit from `IRepositoryBase<Product>` to instantly equip it with a full suite of standard CRUD methods, saving you from having to declare them all manually.