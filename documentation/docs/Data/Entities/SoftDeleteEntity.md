---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.Entities
```

The `SoftDeleteEntity` class is designed to implement the soft-delete pattern for your data entities.
This pattern is a common and valuable technique in application development where, instead of physically removing a record from the database, you simply mark it as deleted.

## Here's a detailed explanation of its purpose:

#### 1. Enabling Soft Deletion:
The primary purpose is to provide an `IsDeleted` boolean flag.
When an entity needs to be "deleted," this flag is set to `true` instead of executing a `DELETE` command on the database.
This means the record remains in the database but is logically considered deleted by the application.

#### 2. Preserving Data History and Audit Trails:
By not physically removing data, you retain a complete history of all records.
This is crucial for auditing, compliance, and debugging, as you can always see what data existed at a certain point in time, even if it was "deleted."

#### 3. Data Recovery: Soft deletion makes it easy to "undelete" or restore records.
If a user accidentally deletes something, or if a business requirement changes, the data can be reactivated by simply setting `IsDeleted` back to `false`.

#### 4. Avoiding Foreign Key Constraints:
Physical deletion can often be complicated by foreign key constraints.
If a record is referenced by other tables, you might have to delete cascading records, which can be risky.
Soft deletion bypasses this issue, as the record still exists and its foreign key relationships remain intact.

#### 5. Consistency with Base Entities:
As `SoftDeleteEntity` inherits from [`BaseEntity`](./BaseEntity.md), it automatically includes the `Id`, `CreatedAt`, and `LastModifiedAt` properties.
This ensures that even soft-deleted records maintain their unique identifier and timestamp information, providing a consistent foundation for all your entities.

## Properties
The `SoftDeleteEntity` class includes its own IsDeleted property and also inherits several key properties from its parent, [`BaseEntity`](./BaseEntity.md).

| Property | Type	| Description | Inherited From |
| -------- | ------ | ----------- | -------------- |
| **Id** | `Guid` |	The unique identifier for the entity.	| [`BaseEntity`](./BaseEntity.md) |
| **CreatedAt** | `DateTime` |	The timestamp of when the entity was created. | [`BaseEntity`](./BaseEntity.md) |
| **LastModifiedAt** | `DateTime?` |	The timestamp of when the entity was last modified.| [`BaseEntity`](./BaseEntity.md) |
| **IsDeleted** | `bool` |	A flag indicating whether the entity is marked as deleted. |	- |

This structure ensures that any entity supporting soft deletion also has the fundamental properties for unique identification and timestamping, providing a robust and consistent data model.

In essence, `SoftDeleteEntity` provides a simple, standardized way to manage the lifecycle of your data without permanently losing information, offering flexibility and robustness to your application's data layer.

## Example: Product Entity
Let's imagine you are creating a `Product` entity for an e-commerce application.
By inheriting from `SoftDeleteEntity`, your `Product` class will automatically gain the ability to be soft-deleted, along with the standard identification and timestamping properties.

```csharp showLineNumbers
using RA.Utilities.Data.Entities;

namespace YourApp.Domain.Entities;

/// <summary>
/// Represents a product in the system.
/// It inherits from SoftDeleteEntity to support soft-deletion.
/// </summary>
public class Product : SoftDeleteEntity
{
    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; set; }
}
```

### What This Gives You
By inheriting from `SoftDeleteEntity`, the `Product` class now has the following properties:

| Property | Type |	Description	| Source |
| -------- | ---- |	----------	| ------ |
| **Id** | `Guid` |	The unique identifier for the product. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **CreatedAt** | `DateTime` |	The timestamp of when the product was created. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **LastModifiedAt** | `DateTime?` |	The timestamp of when the product was last modified. | Inherited from [`BaseEntity`](./BaseEntity.md) |
| **IsDeleted** | `bool` |	A flag indicating whether the product is marked as deleted. |Inherited from `SoftDeleteEntity` |
| **Name** | `string` |	The name of the product. | Defined in `Product` |
| **Description** | `string?` |	The description of the product. | Defined in `Product` |
| **Price** | `decimal` |	The price of the product. |	Defined in `Product` |

This approach allows you to focus on the properties that are unique to your Product entity, while the RA.Utilities.Data.Entities package | provides a consistent and reusable foundation for common data management features like soft deletion and timestamping.