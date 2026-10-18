---
title: ReadRepositoryBase
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Data.EntityFramework
```

`ReadRepositoryBase<T, TKey>` is the EF Core implementation of [`IReadRepositoryBase<T, TKey>`](../Abstractions/IReadRepositoryBase.md). It provides read-only queries for any entity derived from [`CoreEntity<TKey>`](../Entities/CoreEntity.md) and never exposes a method that changes the database.

## 🎯 Purpose

Use this repository on the query side of your application. Depending on the interface instead of the `DbContext` makes the read-only intent explicit, keeps business logic free of EF Core calls, and lets you swap the implementation in tests.

`ListAsync` applies `AsNoTracking()`, so query results do not enter the change tracker. That reduces memory overhead and is the right default for data that will only be displayed or mapped to a DTO.

## Class signature

```csharp showLineNumbers
public class ReadRepositoryBase<T, TKey> : IReadRepositoryBase<T, TKey>
    where T : CoreEntity<TKey>
    where TKey : notnull
```

## Constructor

| Constructor | Description |
|---|---|
| `ReadRepositoryBase(DbContext dbContext)` | Stores the context and resolves `DbContext.Set<T>()`. Throws `ArgumentNullException` when `dbContext` is `null`. |

## Methods

| Method | Returns | Behavior |
|---|---|---|
| `GetByIdAsync(TKey id, CancellationToken cancellationToken = default)` | `Task<T?>` | Uses `DbSet<T>.FindAsync`. Returns `null` when the key does not exist. `FindAsync` returns an already-tracked instance when one exists, so this query is **not** no-tracking. |
| `ListAsync(Expression<Func<T,bool>>? filter = null, Func<IQueryable<T>,IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default, params Expression<Func<T,object>>[] includeProperties)` | `Task<IReadOnlyList<T>>` | Builds a query, applies `Include` for each navigation path, then `Where`, `OrderBy`, `Skip`, and `Take`, and executes it with `AsNoTracking()`. |

### `ListAsync` parameter order

`includeProperties` is a `params` array declared **after** `cancellationToken`. Callers can pass navigation paths positionally after the cancellation token or by name:

```csharp showLineNumbers
// Named arguments read best when several options are combined
await repository.ListAsync(
    filter: p => p.IsActive,
    orderBy: q => q.OrderBy(p => p.Name),
    skip: 0,
    take: 20,
    cancellationToken: cancellationToken,
    includeProperties: [p => p.Category]);

// Positional includes are also valid
await repository.ListAsync(null, null, null, null, cancellationToken, p => p.Category, p => p.Supplier);
```

## Properties

| Member | Type | Description |
|---|---|---|
| `dataContext` | `DbContext` | `protected` field holding the context, available to derived repositories. |

## 🚀 How to Use

:::note[Assumed entity]
The examples below assume `public sealed class Product : CoreEntity<Guid> { /* properties */ }` and a registered `AppDbContext`. See [Getting Started](./index.mdx#hello-world) for the full setup.
:::

### Register the repository

The repositories are open-generic and take a `DbContext`. Register your context under the `DbContext` service type so DI can construct them:

```csharp showLineNumbers
using RA.Utilities.Data.EntityFramework.Extensions;

builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// highlight-next-line
builder.Services.AddReadRepositoryBase();
```

### Query from a service

```csharp showLineNumbers
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

public sealed class ActiveProductReport
{
    private readonly IReadRepositoryBase<Product, Guid> _repository;

    public ActiveProductReport(IReadRepositoryBase<Product, Guid> repository) =>
        _repository = repository;

    public Task<IReadOnlyList<Product>> GetFirstPageAsync(CancellationToken cancellationToken = default) =>
        _repository.ListAsync(
            filter: p => p.IsActive,
            orderBy: q => q.OrderBy(p => p.Name),
            take: 20,
            cancellationToken: cancellationToken);
}
```

## ⚠️ Constraints

- **No projections**: `ListAsync` returns entities. For a DTO projection (`Select`), use the `DbContext` directly or add a custom repository method.
- **No raw SQL / compiled queries**: the base class exposes only the two interface methods.
- **No tracking on lists**: if you need to update entities loaded through `ListAsync`, attach them explicitly or reload them with `GetByIdAsync` (which uses `FindAsync`).
