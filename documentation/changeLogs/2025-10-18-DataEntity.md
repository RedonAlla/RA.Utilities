---
title: RA.Utilities.Data.Entities
authors: [RedonAlla]
---

## Version 10.0.0-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/10.0.0-rc.2)


This is the initial release of `RA.Utilities.Data.Entities`, a library providing core abstractions for data models.
<!-- truncate -->

### ✨ Key Features

*   **`IEntity<T>` Interface**: A core abstraction that defines a contract for any entity with a typed identifier (`Id`).

*   **`IAuditable` Interface**: Defines a contract for entities that need auditing fields, including `CreatedDate` and `UpdatedDate`.

*   **`BaseEntity<T>` Class**: An abstract base class that provides a ready-to-use implementation of `IEntity<T>` and `IAuditable`. It includes:
    *   A typed `Id` property.
    *   `CreatedDate` (DateTime) and `UpdatedDate` (DateTime?) properties for auditing.

*   **`BaseEntity` Class**: A non-generic convenience class that inherits from `BaseEntity<Guid>`, providing a `Guid` as the default primary key type.

*   **Reduced Boilerplate**: By inheriting from these base classes, you can significantly reduce repetitive code in your data models.

### 🚀 Getting Started

To use the package, simply have your entity classes inherit from one of the provided base classes.

#### Example with a specific key type (`int`)
```csharp
public class Product : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

#### Example with the default key type (`Guid`)
```csharp
public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
}
```