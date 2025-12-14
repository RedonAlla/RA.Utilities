---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Data.Entities
```

This is the root abstract class for all entities.
It is meant to be inherited by other entities.
This enforces a pattern where every entity in your system must be built upon this common foundation.
`CoreEntity`: The name itself signifies its role. It is the ***"core"*** or the absolute minimum that any entity in your system must have.

## Properties

| Property | Type   | Description |
| -------- |------- | ----------- |
| Id       | `Guid` | A virtual property for the entity's unique identifier. |

### Its Role in the Entity Hierarchy
The `CoreEntity` class sits at the very top of your entity inheritance chain.
Other, more specialized base classes build upon it.
For example, your [`BaseEntity`](./BaseEntity.md) class inherits from `CoreEntity` to add timestamping properties:

```csharp
public abstract class BaseEntity : CoreEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
```

This creates a clear and logical hierarchy:

* 1. **`CoreEntity`**: Provides the `Id`.
* 1. **`BaseEntity`**: Inherits `Id` and adds `CreatedAt` and `LastModifiedAt`.
* 1. **`AuditableBaseEntity`** or **`SoftDeleteEntity`**: Inherit from `BaseEntity` and add even more specific functionality.

### 🧠 Summary of Purpose
The `CoreEntity` class enforces a fundamental design principle in your data model:

* **Consistency**: Every entity has a primary key with the same name (`Id`) and type (`Guid`).
* **Reusability**: It eliminates the need to declare an `Id` property in every single entity class, adhering to the ***Don't Repeat Yourself (DRY)*** principle.
* **Architectural Foundation**: It serves as the root of the entity hierarchy, providing a stable base upon which more complex entities can be built.

By starting with this simple, abstract class, you create a clean, predictable, and scalable data model for your entire application.