---
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# AuditableBaseEntity&lt;TKey&gt;

`AuditableBaseEntity<TKey>` inherits from [`WriteEntity<TKey>`](./WriteEntity.md) and adds user auditing capabilities to your data models.

While [`WriteEntity<TKey>`](./WriteEntity.md) tracks **when** a record was created or modified (`CreatedAt`, `LastModifiedAt`), `AuditableBaseEntity<TKey>` also tracks **who** performed those actions (`CreatedBy`, `LastModifiedBy`). This provides full accountability for security, compliance, and enterprise audit requirements.

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
| **`CreatedBy`** | `string?` | Gets or sets the identifier of the user who created the entity. | Defined in `AuditableBaseEntity<TKey>` |
| **`LastModifiedBy`** | `string?` | Gets or sets the identifier of the user who last modified the entity. | Defined in `AuditableBaseEntity<TKey>` |

:::info User Identifiers
The `CreatedBy` and `LastModifiedBy` fields are strings, allowing you to store usernames, user emails, subject claims (`sub`), or GUID user identifiers as appropriate for your authentication system.
:::

## Class Definition

```csharp showLineNumbers
using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that track creation and modification user identifiers,
/// inheriting common properties from <see cref="WriteEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class AuditableBaseEntity<TKey> : WriteEntity<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
```

## Usage Example

```csharp showLineNumbers
using System;
using RA.Utilities.Data.Entities;

namespace MyApp.Domain.Entities;

public class Order : AuditableBaseEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
}
```

### Resulting Structure

An instance of `Order` exposes the complete audit and identification trail:

| Property | Type | Role | Origin |
|---|---|---|---|
| **`Id`** | `Guid` | Primary key | Inherited from `CoreEntity<Guid>` |
| **`CreatedAt`** | `DateTime` | Creation time | Inherited from `WriteEntity<Guid>` |
| **`LastModifiedAt`** | `DateTime?` | Update time | Inherited from `WriteEntity<Guid>` |
| **`CreatedBy`** | `string?` | Creator identifier | Inherited from `AuditableBaseEntity<Guid>` |
| **`LastModifiedBy`** | `string?` | Modifier identifier | Inherited from `AuditableBaseEntity<Guid>` |
| **`CustomerId`** | `Guid` | Foreign key | Defined in `Order` |
| **`TotalAmount`** | `decimal` | Order amount | Defined in `Order` |
| **`Status`** | `string` | Order status | Defined in `Order` |
