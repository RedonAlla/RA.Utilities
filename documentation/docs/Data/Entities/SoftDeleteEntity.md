---
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# SoftDeleteEntity&lt;TKey&gt;

`SoftDeleteEntity<TKey>` inherits from [`WriteEntity<TKey>`](./WriteEntity.md) and implements the soft-delete pattern for your data models.

Instead of executing a physical SQL `DELETE` statement that removes records from the database table, entities marked for soft deletion toggle the `IsDeleted` boolean flag to `true`. This preserves relational history, simplifies data restoration, and prevents unintended foreign-key cascading issues.

## Why Use Soft Deletion?

1. **Preserving Audit History**: Retains historical transaction records for regulatory compliance and auditing.
2. **Safe Recovery**: Allows accidentally deleted records to be restored simply by flipping `IsDeleted` back to `false`.
3. **Foreign Key Integrity**: Avoids database foreign-key constraint violations when deleting parent records referenced by other tables.
4. **Lifecycle Timestamps**: Because it inherits from [`WriteEntity<TKey>`](./WriteEntity.md), every soft-deleted entity maintains its `CreatedAt` and `LastModifiedAt` timestamps.

## Type Parameters

| Parameter | Description |
|---|---|
| **`TKey`** | The data type of the entity's primary key identifier. |

## Properties

| Property | Type | Description | Source |
|---|---|---|---|
| **`Id`** | `TKey` | The unique identifier for the entity. | Inherited from [`CoreEntity<TKey>`](./CoreEntity.md) |
| **`CreatedAt`** | `DateTime` | Gets or sets the creation timestamp of the entity. | Inherited from [`WriteEntity<TKey>`](./WriteEntity.md) |
| **`LastModifiedAt`** | `DateTime?` | Gets or sets the last modification timestamp of the entity. | Inherited from [`WriteEntity<TKey>`](./WriteEntity.md) |
| **`IsDeleted`** | `bool` | Gets or sets a value indicating whether the entity is marked as deleted. | Defined in `SoftDeleteEntity<TKey>` |

## Class Definition

```csharp showLineNumbers
namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that support soft deletion, inheriting common properties from <see cref="WriteEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class SoftDeleteEntity<TKey> : WriteEntity<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
```

## Usage Example

```csharp showLineNumbers
using System;
using RA.Utilities.Data.Entities;

namespace MyApp.Domain.Entities;

public class Article : SoftDeleteEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}
```

### EF Core Global Query Filter Integration

To automatically exclude soft-deleted entities from all queries in Entity Framework Core, apply a query filter in `OnModelCreating`:

```csharp showLineNumbers
using Microsoft.EntityFrameworkCore;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Article>()
        // highlight-next-line
        .HasQueryFilter(a => !a.IsDeleted);
}
```

To query soft-deleted records when needed (such as in an admin portal or recycle bin):

```csharp showLineNumbers
var deletedArticles = await dbContext.Articles
    // highlight-next-line
    .IgnoreQueryFilters()
    .Where(a => a.IsDeleted)
    .ToListAsync();
```