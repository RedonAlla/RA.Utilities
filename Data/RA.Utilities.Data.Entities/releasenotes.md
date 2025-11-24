# Release Notes for RA.Utilities.Data.Entities

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/10.0.0)

Updated version from `10.0.0-rc.2` to `10.0.0`, indicating the release candidate phase has ended and the package is now considered stable for production use.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/10.0.0-rc.2)

This release of `RA.Utilities.Data.Entities` provides a set of core abstractions and base classes for data entities. It helps solve the problem of boilerplate and inconsistency in data models by providing common interfaces and base classes with standard properties.

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