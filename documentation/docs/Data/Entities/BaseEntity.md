---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Data.Entities
```

The `BaseEntity` class serves as the foundation for all entities within your data model.
It encapsulates common properties that nearly every entity will require, reducing redundancy and ensuring consistency across your application.

## 🧠 Here's a breakdown of its purpose:

#### 1. Unique Identification:
It provides a standard `Id` property, usually a `Guid`, which serves as the primary key for uniquely identifying each entity instance.

#### 2. Timestamping:
It includes `CreatedAt` and `LastModifiedAt` properties.
These properties automatically record when an entity was initially created and when it was last updated, providing valuable historical context for data changes.

## Properties

| Property   | Type        | Description                                         | Inherited From |
|------------|-------------|-----------------------------------------------------| -------------- |
| Id         | `Guid`      | The unique identifier for the entity.               | [`CoreEntity`](./CoreEntity.md) |
| CreatedAt  | `DateTime`  | The date and time when the entity was created.      | - |
| ModifiedAt | `DateTime?` | The date and time when the entity was last modified.| - |


By inheriting from `BaseEntity`, you automatically equip your entities with these essential properties, promoting code reuse and simplifying data management.

## Example: Category Entity

By inheriting from `BaseEntity`, the Category class automatically gets the essential properties for a unique identifier and timestamps, which saves you from writing repetitive code.

```csharp showLineNumbers
using RA.Utilities.Data.Entities;

namespace YourApp.Domain.Entities;

/// <summary>
/// Represents a product category in the system.
/// It inherits from BaseEntity to get standard properties like Id and timestamps.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description for the category.
    /// </summary>
    public string? Description { get; set; }
}
```

### Resulting Properties
When you create the `Category` entity as shown above, it will have the following properties. The ones from `BaseEntity` are inherited automatically.

| Property | Type |	Description	| Source |
| -------- | ---- |	-----------	| ------ |
| **Id** | `Guid` |	The unique identifier for the category.	| Inherited from `BaseEntity` |
| **CreatedAt** | `DateTime` |	The timestamp of when the category was created.	| Inherited from `BaseEntity` |
| **LastModifiedAt** | `DateTime?` |	The timestamp of when the category was last modified. |	Inherited from `BaseEntity` |
| **Name** | `string` |	The name of the category.	| Defined in `Category` |
| **Description** | `string?` |	The description of the category.	| Defined in `Category` |

This approach allows you to build your data models on a consistent and solid foundation, focusing only on the properties that are unique to each entity.