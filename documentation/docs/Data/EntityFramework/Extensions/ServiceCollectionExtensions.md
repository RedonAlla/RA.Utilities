---
title: ServiceCollectionExtensions
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Extensions
```

`ServiceCollectionExtensions` registers the open-generic repositories in the dependency injection container. Each method is a one-line replacement for a manual `AddScoped(typeof(...), typeof(...))` call.

:::info[v10.1.0 rename]
This class was renamed from `DependencyInjectionExtensions` in v10.1.0 and its methods are now declared as C# 14 extension members. Extension-method call sites (`builder.Services.AddRepositoryBase()`) are unchanged; only code that references the class by name, such as `typeof(DependencyInjectionExtensions)` or a static call, needs updating.
:::

## Methods

| Method | Service type | Implementation | Lifetime |
|---|---|---|---|
| `AddReadRepositoryBase()` | `IReadRepositoryBase<,>` | `ReadRepositoryBase<,>` | Scoped |
| `AddWriteRepositoryBase()` | `IWriteRepositoryBase<,>` | `WriteRepositoryBase<,>` | Scoped |
| `AddRepositoryBase()` | `IRepositoryBase<,>` | `RepositoryBase<,>` | Scoped |

All registrations are **open generic**: one call covers every entity `T` that derives from [`CoreEntity<TKey>`](../../Entities/CoreEntity.md), with any non-nullable key type `TKey`.

## ⚠️ Register the `DbContext` correctly

The repository constructors take a `DbContext`, not your concrete context type. `AddDbContext<TContext>()` registers the context under `TContext` only, so `IRepositoryBase<Product, Guid>` cannot be resolved unless `DbContext` itself is registered.

**Recommended — register the context as `DbContext`:**

```csharp showLineNumbers
using RA.Utilities.Data.EntityFramework.Extensions;

builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// highlight-next-line
builder.Services.AddRepositoryBase();
```

**Alternative — alias an existing registration:**

```csharp showLineNumbers
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddRepositoryBase();
```

If you use custom repositories that accept `AppDbContext` directly, you do not need the open-generic registrations at all. See [Troubleshooting](../troubleshooting.mdx#unable-to-resolve-service-for-type-microsoftentityframeworkcoredbcontext).


:::note[Assumed entity]
The examples assume `public sealed class Product : CoreEntity<Guid> { /* properties */ }` and an `AppDbContext` registered as shown above.
:::
## `AddReadRepositoryBase`

Registers the read-only repository for services that only query data.

```csharp showLineNumbers
builder.Services.AddReadRepositoryBase();
```

```csharp showLineNumbers
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

public sealed class ProductReportService
{
    private readonly IReadRepositoryBase<Product, Guid> _repository;

    public ProductReportService(IReadRepositoryBase<Product, Guid> repository) =>
        _repository = repository;

    public Task<IReadOnlyList<Product>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        _repository.ListAsync(filter: p => p.IsActive, cancellationToken: cancellationToken);
}
```

## `AddWriteRepositoryBase`

Registers the write-only repository for services that create, update, or delete data.

```csharp showLineNumbers
builder.Services.AddWriteRepositoryBase();
```

```csharp showLineNumbers
public sealed class ProductCommandService
{
    private readonly IWriteRepositoryBase<Product, Guid> _repository;

    public ProductCommandService(IWriteRepositoryBase<Product, Guid> repository) =>
        _repository = repository;

    public async Task CreateAsync(string name, decimal price, CancellationToken cancellationToken = default)
    {
        var product = new Product { Name = name, Price = price };

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
```

## `AddRepositoryBase`

Registers the full repository for services that both query and modify the same entity.

```csharp showLineNumbers
builder.Services.AddRepositoryBase();
```

```csharp showLineNumbers
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
        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
```

## Choosing what to register

| Inject | Register |
|---|---|
| `IReadRepositoryBase<T, TKey>` | `AddReadRepositoryBase()` |
| `IWriteRepositoryBase<T, TKey>` | `AddWriteRepositoryBase()` |
| `IRepositoryBase<T, TKey>` | `AddRepositoryBase()` |

Register only what you inject. Registering all three is harmless, but each entity then has three scoped repository instances resolving the same scoped `DbContext`.

## Lifetime

Every registration is scoped. That matches the default scoped lifetime of `AddDbContext` and keeps one `DbContext` per request/scope, so the repositories and the unit of work share a change tracker.

Do not resolve these repositories from a singleton, and do not capture one in a background task that outlives the request scope.
