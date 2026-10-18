---
title: RepositoryBase
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

`RepositoryBase<T, TKey>` is the full-featured repository: it implements [`IRepositoryBase<T, TKey>`](../Abstractions/IRepositoryBase.md), which combines the read and write contracts.

## 🎯 Purpose

Use it when a service needs both queries and commands for the same entity and splitting them into separate services would add indirection without benefit. When reads and writes should stay apart, inject [`IReadRepositoryBase<T, TKey>`](./ReadRepositoryBase.md) and [`IWriteRepositoryBase<T, TKey>`](./WriteRepositoryBase.md) instead.

## How it composes behavior

`RepositoryBase<T, TKey>` inherits reads and composes writes:

```csharp showLineNumbers
public class RepositoryBase<T, TKey> : ReadRepositoryBase<T, TKey>, IRepositoryBase<T, TKey>
    where T : CoreEntity<TKey>
    where TKey : notnull
{
    private readonly WriteRepositoryBase<T, TKey> _writeRepository;

    public RepositoryBase(DbContext dbContext) : base(dbContext)
    {
        _writeRepository = new WriteRepositoryBase<T, TKey>(dbContext);
    }

    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        => _writeRepository.AddAsync(entity, cancellationToken);

    // ... the remaining write methods delegate the same way
}
```

```
RepositoryBase<T, TKey>
├─ inherits  ReadRepositoryBase<T, TKey>   → GetByIdAsync, ListAsync
└─ composes  WriteRepositoryBase<T, TKey>  → AddAsync, AddRangeAsync,
                                              UpdateAsync, UpdateRangeAsync,
                                              DeleteAsync, DeleteRangeAsync,
                                              SaveChangesAsync
```

Both halves receive the **same** `DbContext` instance, so reads and writes share one change tracker and one unit of work.

This is the **composition over inheritance** approach: instead of duplicating read and write code in a deep hierarchy, the write behavior is delegated to a dedicated object.

## Constructor

| Constructor | Description |
|---|---|
| `RepositoryBase(DbContext dbContext)` | Passes the context to `ReadRepositoryBase` and creates the composed `WriteRepositoryBase` with the same instance. |

## Members

| Member | Source | Description |
|---|---|---|
| `GetByIdAsync`, `ListAsync` | Inherited from `ReadRepositoryBase<T, TKey>` | Query methods; `ListAsync` is no-tracking. |
| `AddAsync`, `AddRangeAsync`, `UpdateAsync`, `UpdateRangeAsync`, `DeleteAsync`, `DeleteRangeAsync`, `SaveChangesAsync` | Delegated to the composed `WriteRepositoryBase<T, TKey>` | Command methods; all are `virtual`. |

See [`ReadRepositoryBase`](./ReadRepositoryBase.md) and [`WriteRepositoryBase`](./WriteRepositoryBase.md) for parameter and return details.

## 🚀 How to Use

:::note[Assumed entity]
The examples below assume `public sealed class Product : CoreEntity<Guid> { /* properties */ }` and a registered `AppDbContext`. See [Getting Started](./index.mdx#hello-world) for the full setup.
:::

### Register

```csharp showLineNumbers
using RA.Utilities.Data.EntityFramework.Extensions;

builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// highlight-next-line
builder.Services.AddRepositoryBase();
```

### Use in a service

```csharp showLineNumbers
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

public sealed class ProductService
{
    private readonly IRepositoryBase<Product, Guid> _repository;

    public ProductService(IRepositoryBase<Product, Guid> repository) => _repository = repository;

    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public async Task UpdatePriceAsync(Guid id, decimal price, CancellationToken cancellationToken = default)
    {
        Product? product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return;
        }

        product.Price = price;

        // highlight-start
        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        // highlight-end
    }
}
```

## 🛠️ When to use it

| Situation | Prefer |
|---|---|
| A service reads and writes the same entity | `RepositoryBase<T, TKey>` / `IRepositoryBase<T, TKey>` |
| Reads and writes are separate services (CQS) | `ReadRepositoryBase` + `WriteRepositoryBase` |
| Only reporting / querying | `ReadRepositoryBase` |
| Only processing commands | `WriteRepositoryBase` |
| Need DTO projections, raw SQL, or bulk updates | A custom repository deriving from `RepositoryBase<T, TKey>` and using `DbContext` directly |
