# Release Notes for RA.Utilities.Data.Entities

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-19%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/10.1.0)

This release introduces generic primary key support across all entity base classes, establishes a new `WriteEntity<TKey>` abstraction for mutable entities, and refines the inheritance hierarchy to cleanly separate creation-only entities from mutable and auditable entities.

### ⚠️ Breaking Changes

*   **Generic Primary Key (`TKey`)**: All entity base classes (`CoreEntity<TKey>`, `BaseEntity<TKey>`, `WriteEntity<TKey>`, `AuditableBaseEntity<TKey>`, `SoftDeleteEntity<TKey>`) are now generic over `TKey` instead of using a hardcoded `Guid`. Existing entities must specify their key type (e.g., `BaseEntity<Guid>`).
*   **`BaseEntity<TKey>` Streamlined**: `LastModifiedAt` has been removed from `BaseEntity<TKey>` to optimize it for immutable or append-only records that only track creation. For entities that require modification tracking, inherit from `WriteEntity<TKey>`.
*   **Inheritance Hierarchy Realignment**: `AuditableBaseEntity<TKey>` and `SoftDeleteEntity<TKey>` now inherit from `WriteEntity<TKey>` instead of `BaseEntity`.

### ✨ New Features

*   **`WriteEntity<TKey>` Base Class**: Introduced a dedicated base class inheriting from `CoreEntity<TKey>` that provides both `CreatedAt` (`DateTime`) and `LastModifiedAt` (`DateTime?`) timestamps for entities that can be updated.
*   **Custom Key Support**: Full flexibility to use any identifier type (e.g., `Guid`, `int`, `long`, `string`).

### 📝 Improvements

*   **Standardized XML Documentation**: Added complete XML documentation comments across all base entity classes and properties, including `<typeparam name="TKey">` tags to enhance developer experience in IDEs.

### 🚀 Getting Started (Updated)

#### Example: Creation-Only Entity (`BaseEntity<TKey>`)
```csharp
public class AuditLog : BaseEntity<long>
{
    public string Action { get; set; } = string.Empty;
}
```

#### Example: Mutable Entity with Timestamps (`WriteEntity<TKey>`)
```csharp
public class Product : WriteEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

#### Example: Auditable Entity (`AuditableBaseEntity<TKey>`)
```csharp
public class Order : AuditableBaseEntity<Guid>
{
    public decimal TotalAmount { get; set; }
}
```

---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-14%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/10.0.1)

This release introduces a major architectural refactoring of the base entity classes to provide a more flexible and intuitive inheritance hierarchy.

### ✨ Key Features

*   **New `CoreEntity` Root Class**:
    *   A new abstract base class, `CoreEntity`, has been introduced as the root of the hierarchy. It provides a single virtual `Id` property of type `Guid`.

### 🚀 Getting Started (Updated)

To use the package, inherit from the base class that best fits your entity's needs.

#### Example: Basic Entity with Timestamps
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```
This entity will automatically have `Id`, `CreatedAt`, and `LastModifiedAt`.

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