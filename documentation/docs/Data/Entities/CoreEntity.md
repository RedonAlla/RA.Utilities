---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# CoreEntity&lt;TKey&gt;

`CoreEntity<TKey>` is the root abstract class for all entities within the `RA.Utilities.Data.Entities` package. It defines the foundational contract for unique identity across your data model.

By making identity generic via `TKey`, this class allows entities to use whatever identifier type best fits the domain or database schema (such as `Guid`, `int`, `long`, or `string`), while still enforcing a consistent naming and property convention across the entire application.

## Type Parameters

| Parameter | Description |
|---|---|
| **`TKey`** | The data type of the entity's primary key identifier. |

## Properties

| Property | Type | Accessors | Description |
|---|---|---|---|
| **`Id`** | `TKey` | `get; protected set;` | The unique identifier for the entity. |

:::info Protected Setter
The `Id` property uses a `protected set;` accessor. This protects the identifier from unintentional external mutation while allowing Entity Framework Core, object-relational mappers, and derived constructors to populate the key value.
:::

## Class Definition

```csharp showLineNumbers
namespace RA.Utilities.Data.Entities;

/// <summary>
/// Represents the base class for all entities, providing a unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class CoreEntity<TKey>
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public virtual TKey Id { get; protected set; }
}
```

## Role in the Entity Hierarchy

`CoreEntity<TKey>` sits at the root of the inheritance chain:

```
CoreEntity<TKey>
├── BaseEntity<TKey>          (adds CreatedAt)
└── WriteEntity<TKey>         (adds CreatedAt, LastModifiedAt)
    ├── SoftDeleteEntity<TKey> (adds IsDeleted)
    └── AuditableBaseEntity<TKey> (adds CreatedBy, LastModifiedBy)
```

## Usage Example

```csharp showLineNumbers
using RA.Utilities.Data.Entities;

public class Country : CoreEntity<string>
{
    public Country(string isoCode, string name)
    {
        Id = isoCode; // e.g. "US", "DE"
        Name = name;
    }

    public string Name { get; set; }
}
```