<p align="center">
  <img src="../../Assets/Images//entity.svg" alt="RA.Utilities.Data.Entities Logo" width="128">
</p>

# RA.Utilities.Data.Entities

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Entities.svg)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)

Provides a set of core abstractions and base classes for data entities within the **`RA.Utilities`** ecosystem.

## Overview

This package offers common interfaces and base classes to promote consistency and reduce boilerplate code in your data models. By using these core abstractions, you can ensure that all your entities share a common structure, including properties for identifiers and timestamps.

The main benefits are:
- **Consistency**: Enforces a standard structure for all data entities (e.g., `Id`, `CreatedDate`, `UpdatedDate`).
- **Reduced Boilerplate**: Eliminates the need to repeatedly define common properties in every entity class.
- **Generic Repository Support**: The common `IEntity` interface makes it easier to implement generic repository patterns.

## 🛠️ Installation

You can install the package from NuGet.

```shell
dotnet add package RA.Utilities.Data.Entities
```

## Features

This package provides the following core components:

### `IEntity`

A generic interface that defines the contract for an entity with a strongly-typed primary key.

```csharp
public interface IEntity<T>
{
    T Id { get; set; }
}
```

**Properties**

| Property   | Type        | Description                                         |
|------------|-------------|-----------------------------------------------------|
| Id         | `Guid`      | The unique identifier for the entity.               |
| CreatedAt  | `DateTime`  | The date and time when the entity was created.      |
| ModifiedAt | `DateTime?` | The date and time when the entity was last modified.|

### `AuditableBaseEntity`

Inherits from `BaseEntity` and adds properties to track which user created and last modified the entity.

**Properties**

| Property       | Type     | Description                                             |
|----------------|----------|---------------------------------------------------------|
| CreatedBy      | `string` | The identifier of the user who created the entity.      |
| LastModifiedBy | `string` | The identifier of the user who last modified the entity.|

**Example**

```csharp
// Example usage
public class Product : AuditableBaseEntity
{
  public string Name { get; set; }
  public decimal Price { get; set; }
}

// Base class provides:
// public T Id { get; set; }
// public DateTime CreatedDate { get; set; }
// public DateTime? UpdatedDate { get; set; }
```

### `SoftDeleteEntity`

Inherits from `BaseEntity` and adds a flag for implementing soft-delete functionality, allowing entities to be marked as deleted without being permanently removed from the database.

**Properties**

| Property  | Type   | Description                                             |
|-----------|--------|---------------------------------------------------------|
| IsDeleted | `bool` | A boolean value indicating whether the entity is marked as deleted.      |

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please refer to the contribution guidelines in the other project READMEs for the pull request process and coding standards.

Thank you for contributing!