# RA.Utilities.Data.Entities

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Entities?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.Entities.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/Entities/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

This package provides a set of abstract base classes for data entities within the RA.Utilities ecosystem. It helps solve the problem of boilerplate and inconsistency in data models by providing a clear inheritance structure with standard properties like `Id`, `CreatedAt`, and `LastModifiedAt`.

The primary goal is to promote consistency and reduce repetitive code when creating data entities for use with an ORM like Entity Framework Core.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.Entities
```

Or through the NuGet Package Manager in Visual Studio.

## ✨ Features & Hierarchy
The package provides a clear inheritance hierarchy for your entities.
You can choose the base class that best fits your needs.

### 1. CoreEntity
This is the root abstract class for all entities.
It provides a single property.

| Property | Type   | Description |
| -------- |------- | ----------- |
| Id       | `Guid` | A virtual property for the entity's unique identifier. |

### 2. BaseEntity
Inherits from `CoreEntity` and adds timestamp auditing fields.
This is a great starting point for most entities. 

| Property | Type   | Description | Source |
| -------- |------- | ----------- | ------ |
| **Id** | `Guid` |	The unique identifier for the category.	| Inherited from `CoreEntity` |
| CreatedAt  | `DateTime`  | The date and time when the entity was created. | |
| ModifiedAt | `DateTime?` | The date and time when the entity was last modified. | |

### 3. SoftDeleteEntity
Inherits from `BaseEntity` and adds support for soft deletion.
Instead of permanently deleting a record, you can mark it as deleted.

| Property | Type   | Description | Source |
| -------- |------- | ----------- | ------ |
| **Id** | `Guid` |	The unique identifier for the category.	| Inherited from `BaseEntity` |
| CreatedAt  | `DateTime`  | The date and time when the entity was created. | Inherited from `BaseEntity` |
| ModifiedAt | `DateTime?` | The date and time when the entity was last modified. | Inherited from `BaseEntity` |
| IsDeleted | `bool` | A flag to indicate if the entity is considered deleted. | |

### 4. AuditableBaseEntity
Inherits from `BaseEntity` and adds properties to track which user created or modified the entity.

| Property | Type   | Description | Source |
| -------- |------- | ----------- | ------ |
| **Id** | `Guid` |	The unique identifier for the category.	| Inherited from `BaseEntity` |
| CreatedAt  | `DateTime`  | The date and time when the entity was created. | Inherited from `BaseEntity` |
| ModifiedAt | `DateTime?` | The date and time when the entity was last modified. | Inherited from `BaseEntity` |
| CreatedBy | `string?` | Identifier for the user who created the entity. |  |
| LastModifiedBy | `string?` | Identifier for the user who last modified the entity.  |  |

## 🚀 Usage Examples

To use the package, have your entity classes inherit from one of the provided base classes. 

### Example 1: Basic Entity,
For a simple entity that only needs an ID and timestamps, use `BaseEntity`.

```csharp
using RA.Utilities.Data.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```

When you create an instance of this `Product` class, it will have the following properties:

*   `Id` (int)
*   `CreatedDate` (DateTime)
*   `UpdatedDate` (DateTime?)
*   `Name` (string)
*   `Description` (string?)
*   `Price` (decimal)


### Example 2: Soft-Deletable and Auditable Entity
If you need an entity that supports both soft-deletion and user auditing, you can create a new base class that combines `SoftDeleteEntity` and `AuditableBaseEntity` features. 
First, define a combined base class:

This is a non-generic version that provides a `Guid` as the primary key type.

```csharp
using RA.Utilities.Data.Entities;

/// <summary>
/// Represents an entity that supports both soft deletion and user auditing.
/// </summary>
public abstract class AuditableSoftDeleteEntity : AuditableBaseEntity
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
```

Then, inherit from your new base class:
```csharp
public class Order : AuditableSoftDeleteEntity
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid CustomerId { get; set; }
}
```

This Order entity now includes `Id`, `CreatedAt`, `LastModifiedAt`, `CreatedBy`, `LastModifiedBy`, and `IsDeleted`.

### Using with Entity Framework Core

These base entities work seamlessly with EF Core. You can configure the properties in your `DbContext`.


```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });
    }
}
```

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main [officiary documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/Entities/).

## Feedback

If you have suggestions or find a bug, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities).
Contributions are welcome!