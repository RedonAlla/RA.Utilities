---
title: BaseEntitySaveChangesInterceptor
sidebar_position: 7
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Interceptors
```

`BaseEntitySaveChangesInterceptor` sets audit timestamps immediately before EF Core writes changes to the database. It is a `SaveChangesInterceptor` that overrides both `SavingChanges` and `SavingChangesAsync`.

## 🎯 Purpose

Audit fields are easy to forget and easy to set inconsistently. The interceptor centralizes the logic: whenever an entity is added, `CreatedAt` is set to `DateTime.UtcNow`; whenever an entity is modified, `LastModifiedAt` is set to `DateTime.UtcNow`. No repository or business code has to remember.

## How it works

1. EF Core calls `SavingChanges` (sync) or `SavingChangesAsync` (async).
2. The interceptor enumerates `context.ChangeTracker.Entries()`.
3. For every entry in the `Added` state, it checks the entity's model metadata for a `CreatedAt` property and sets it.
4. For every entry in the `Modified` state, it checks for `LastModifiedAt` and, when present, sets it.

The check is **metadata-based** (`entry.Metadata.FindProperty(...)`), not type-based. Any entity that exposes a property with those names is updated, whether or not it derives from a RA.Utilities entity base class.

```csharp showLineNumbers
foreach (EntityEntry entry in context.ChangeTracker.Entries())
{
    if (entry.State is EntityState.Added && entry.Metadata.FindProperty(CreatedAtProperty) is not null)
    {
        entry.Property(CreatedAtProperty).CurrentValue = DateTime.UtcNow;
    }

    if (entry.State is EntityState.Modified && entry.Metadata.FindProperty(LastModifiedAtProperty) is not null)
    {
        entry.Property(LastModifiedAtProperty).CurrentValue = DateTime.UtcNow;
    }
}
```

## Which entities are affected

| Entity base | `CreatedAt` | `LastModifiedAt` |
|---|---|---|
| [`CoreEntity<TKey>`](../../Entities/CoreEntity.md) | ❌ | ❌ |
| [`BaseEntity<TKey>`](../../Entities/BaseEntity.md) | ✅ | ❌ |
| [`WriteEntity<TKey>`](../../Entities/WriteEntity.md) | ✅ | ✅ |
| [`AuditableBaseEntity<TKey>`](../../Entities/AuditableBaseEntity.md) | ✅ | ✅ (plus `CreatedBy` / `LastModifiedBy`, **not** set by this interceptor) |
| [`SoftDeleteEntity<TKey>`](../../Entities/SoftDeleteEntity.md) | ✅ | ✅ (`IsDeleted` is **not** managed by this interceptor) |

The interceptor writes `DateTime.UtcNow` for both properties. It does not use the database clock.

## 🚀 How to Use

Register the interceptor and attach it to the context's options:

```csharp showLineNumbers
using RA.Utilities.Data.EntityFramework.Interceptors;

builder.Services.AddScoped<BaseEntitySaveChangesInterceptor>();

builder.Services.AddDbContext<DbContext, AppDbContext>((provider, options) =>
    options.UseSqlServer(connectionString)
           // highlight-next-line
           .AddInterceptors(provider.GetRequiredService<BaseEntitySaveChangesInterceptor>()));
```

For a context constructed without DI, attach it directly:

```csharp showLineNumbers
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(connectionString)
        // highlight-next-line
        .AddInterceptors(new BaseEntitySaveChangesInterceptor());
}
```

## ⚠️ Behavior and limitations

- **Only `Added` and `Modified`** — `Deleted` entities keep their timestamps, and `Unchanged` entities are untouched.
- **Exact property names** — the properties must be named `CreatedAt` and `LastModifiedAt` to be detected.
- **Timestamps only** — `CreatedBy` and `LastModifiedBy` are not populated; read the current user and set them in your command handler or a separate interceptor.
- **Soft delete is not applied** — `SoftDeleteEntity<TKey>.IsDeleted` is not queried or filtered automatically. Add a global query filter and set the flag in your own code.
- **`DateTime`, not `DateTimeOffset`** — the properties are `DateTime`; values are UTC.
- **Pooled contexts** — if you use `AddDbContextPool`, the interceptor is stateless, so it can safely be registered as a singleton instead of scoped:

  ```csharp
  builder.Services.AddSingleton<BaseEntitySaveChangesInterceptor>();
  ```
