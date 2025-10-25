---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.Entities
```

The primary purpose of the `AuditableBaseEntity` class is to extend the functionality of [`BaseEntity`](./BaseEntity.md) by adding **auditing capabilities**.
While [`BaseEntity`](./BaseEntity.md) tracks when a record was created or modified, `AuditableBaseEntity` also tracks who performed those actions.

This is essential for applications that require accountability, security, and a clear audit trail for data changes.


## 🧠 Here's a breakdown of its purpose:

#### 1. Unique Identification:
It provides a standard `Id` property, usually a `Guid`, which serves as the primary key for uniquely identifying each entity instance.

#### 2. Timestamping:
It includes `CreatedAt` and `LastModifiedAt` properties.
These properties automatically record when an entity was initially created and when it was last updated, providing valuable historical context for data changes.
By using `AuditableBaseEntity`, you can easily answer questions like "Who created this customer record?" or "Who was the last person to update this order?".

## Properties
The `AuditableBaseEntity class includes its own `CreatedAt` and `LastModifiedAt` properties,
and also inherits several key properties from its parent, [`BaseEntity`](./BaseEntity.md).

| Property | Type |	Description	| Source |
| -------- | ---- |	----------	| ------ |
| **Id** | `Guid` |	The unique identifier for the entity. | [`BaseEntity`](./BaseEntity.md) |
| **CreatedAt** | `DateTime` |	The timestamp of when the entity was created. | [`BaseEntity`](./BaseEntity.md) |
| **LastModifiedAt** | `DateTime?` |	The timestamp of when the entity was last modified.	| [`BaseEntity`](./BaseEntity.md) |
| **CreatedBy** | `string` |	The identifier of the user who created the entity. | - |
| **LastModifiedBy** | `string?` |	The identifier of the user who last modified the entity. | - |

This structure ensures that any entity requiring auditing capabilities also has the fundamental properties for unique identification and timestamping, providing a robust and consistent data model.

## Example: `Order` Entity

When you need to track which user created or last modified a record, inheriting from `AuditableBaseEntity` is the perfect solution.
It provides all the standard properties from [`BaseEntity`](./BaseEntity.md) and adds the necessary auditing fields.

Let's create an `Order` entity for an e-commerce system as an example.



```csharp showLineNumbers
using System;
using RA.Utilities.Data.Entities;

namespace YourApp.Domain.Entities;

/// <summary>
/// Represents a customer order in the system.
/// It inherits from AuditableBaseEntity to track creation and modification by users.
/// </summary>
public class Order : AuditableBaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the customer who placed the order.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the date the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the order.
    /// </summary>
    public decimal TotalAmount { get; set; }
}
```

### Resulting Properties
By inheriting from `AuditableBaseEntity`, the `Order` class is automatically equipped with properties for identification, timestamping, and auditing.
Here is a complete list of its properties:

| Property | Type |	Description	| Source |
| -------- | ---- |	-----------	| ------ |
| **Id** | `Guid` |	The unique identifier for the product. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **CreatedAt** | `DateTime` |	The timestamp of when the product was created. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **LastModifiedAt** | `DateTime?` | The timestamp of when the product was last modified. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **CreatedBy** | `string` |	The identifier of the user who created the order. |	Inherited from `AuditableBaseEntity` |
| **LastModifiedBy** | `string?` |	The identifier of the user who last modified the order. | Inherited from `AuditableBaseEntity` |
| **CustomerId** | `Guid` | The ID of the customer who placed the order. |	Defined in `Order` |
| **OrderDate** | `DateTime	` | The date the order was placed. | Defined in `Order` |
| **TotalAmount** | `decimal` |	The total amount for the order. | Defined in `Order` |
