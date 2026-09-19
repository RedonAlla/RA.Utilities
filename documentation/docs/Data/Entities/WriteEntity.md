---
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# WriteEntity&lt;TKey&gt;

`WriteEntity<TKey>` inherits from [`CoreEntity<TKey>`](./CoreEntity.md) and represents mutable domain and data entities. It provides timestamp tracking for both initial creation (`CreatedAt`) and subsequent modifications (`LastModifiedAt`).

This class is the standard, recommended starting point for general-purpose entities in your application (products, customers, orders, categories) that can be created and edited over time.

## Type Parameters

| Parameter | Description |
|---|---|
| **`TKey`** | The data type of the entity's primary key identifier. |

## Properties

| Property | Type | Description | Source |
|---|---|---|---|
| **`Id`** | `TKey` | The unique identifier for the entity. | Inherited from [`CoreEntity<TKey>`](./CoreEntity.md) |
| **`CreatedAt`** | `DateTime` | Gets or sets the creation timestamp of the entity. | Defined in `WriteEntity<TKey>` |
| **`LastModifiedAt`** | `DateTime?` | Gets or sets the last modification timestamp of the entity. | Defined in `WriteEntity<TKey>` |

:::info Nullable LastModifiedAt
`LastModifiedAt` is a nullable `DateTime?`. When an entity is first created, this property is `null`. It is populated with the current timestamp whenever the entity is subsequently updated.
:::

## Class Definition

```csharp showLineNumbers
using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for writable entities that track creation and modification timestamps,
/// inheriting the unique identifier from <see cref="CoreEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class WriteEntity<TKey> : CoreEntity<TKey>
{
    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp of the entity.
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
}
```

## Role in the Entity Hierarchy

`WriteEntity<TKey>` serves as the parent class for more specialized lifecycle entities:

```
WriteEntity<TKey> (CreatedAt, LastModifiedAt)
├── SoftDeleteEntity<TKey>    (adds IsDeleted)
└── AuditableBaseEntity<TKey> (adds CreatedBy, LastModifiedBy)
```

## Usage Example

```csharp showLineNumbers
using System;
using RA.Utilities.Data.Entities;

namespace MyApp.Domain.Entities;

public class Product : WriteEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```

### Resulting Structure

When you create an instance of `Product`, it possesses the following properties:

| Property | Type | Description | Origin |
|---|---|---|---|
| **`Id`** | `Guid` | Unique entity identifier | Inherited from `CoreEntity<Guid>` |
| **`CreatedAt`** | `DateTime` | Entity creation timestamp | Inherited from `WriteEntity<Guid>` |
| **`LastModifiedAt`** | `DateTime?` | Entity modification timestamp (`null` until updated) | Inherited from `WriteEntity<Guid>` |
| **`Name`** | `string` | Name of the product | Defined in `Product` |
| **`Description`** | `string?` | Optional product description | Defined in `Product` |
| **`Price`** | `decimal` | Unit price | Defined in `Product` |
