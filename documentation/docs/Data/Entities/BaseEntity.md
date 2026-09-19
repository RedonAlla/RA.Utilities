---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# BaseEntity&lt;TKey&gt;

`BaseEntity<TKey>` inherits from [`CoreEntity<TKey>`](./CoreEntity.md) and provides creation timestamp tracking via the `CreatedAt` property.

This class is purpose-built for **append-only** or **immutable** entities—such as audit logs, event stream records, notifications, or financial ledger transactions—where records are written once upon creation and are never updated or modified.

:::tip Need Modification Tracking?
If your entity is mutable and needs to record when it was updated, use [`WriteEntity<TKey>`](./WriteEntity.md) instead of `BaseEntity<TKey>`.
:::

## Type Parameters

| Parameter | Description |
|---|---|
| **`TKey`** | The data type of the entity's primary key identifier. |

## Properties

| Property | Type | Description | Source |
|---|---|---|---|
| **`Id`** | `TKey` | The unique identifier for the entity. | Inherited from [`CoreEntity<TKey>`](./CoreEntity.md) |
| **`CreatedAt`** | `DateTime` | Gets or sets the creation timestamp of the entity. | Defined in `BaseEntity<TKey>` |

## Class Definition

```csharp showLineNumbers
using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities, providing a creation timestamp and
/// inheriting the unique identifier from <see cref="CoreEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class BaseEntity<TKey> : CoreEntity<TKey>
{
    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
```

## Usage Example

The following example demonstrates an immutable event log entity that tracks system activity:

```csharp showLineNumbers
using System;
using RA.Utilities.Data.Entities;

namespace MyApp.Domain.Entities;

public class SecurityEvent : BaseEntity<long>
{
    public string EventType { get; set; } = string.Empty;
    public string UserIpAddress { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}
```

### Resulting Structure

When an instance of `SecurityEvent` is created, it exposes:
* `Id` (`long`) — Unique record identifier inherited from `CoreEntity<long>`.
* `CreatedAt` (`DateTime`) — Creation timestamp inherited from `BaseEntity<long>`.
* `EventType` (`string`) — Specific event payload.
* `UserIpAddress` (`string`) — Client IP address.
* `Details` (`string`) — Additional metadata.