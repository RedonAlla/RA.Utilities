# RA.Utilities.Data.Entities

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Entities?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.Entities.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/Entities/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

This package provides a set of abstract base classes for data entities within the RA.Utilities ecosystem.
It helps solve the problem of boilerplate and inconsistency in data models by providing a clear generic inheritance structure with standard properties like `Id`, `CreatedAt`, `LastModifiedAt`, and audit metadata.

The primary goal is to promote consistency, type safety, and code reuse when creating data entities for use with an ORM like Entity Framework Core.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.Entities
```

Or through the NuGet Package Manager in Visual Studio.

## ✨ Features & Hierarchy

The package provides a clear inheritance hierarchy for your entities, supporting any primary key type (`Guid`, `int`, `long`, `string`, etc.) via the generic parameter `TKey`.

```
CoreEntity<TKey>
├── BaseEntity<TKey>
└── WriteEntity<TKey>
    ├── SoftDeleteEntity<TKey>
    └── AuditableBaseEntity<TKey>
```

### 1. CoreEntity&lt;TKey&gt;
The root abstract class for all entities. It provides a strongly typed unique identifier.

| Property | Type | Description |
| -------- | ---- | ----------- |
| `Id` | `TKey` | A virtual property for the entity's unique identifier. |

### 2. BaseEntity&lt;TKey&gt;
Inherits from `CoreEntity<TKey>` and adds creation timestamp tracking. This is ideal for immutable, append-only, or event log entities.

| Property | Type | Description | Source |
| -------- | ---- | ----------- | ------ |
| `Id` | `TKey` | The unique identifier for the entity. | Inherited from `CoreEntity<TKey>` |
| `CreatedAt` | `DateTime` | The date and time when the entity was created. | Defined in `BaseEntity<TKey>` |

### 3. WriteEntity&lt;TKey&gt;
Inherits from `CoreEntity<TKey>` and adds both creation and modification timestamps. This is the recommended starting point for standard mutable entities.

| Property | Type | Description | Source |
| -------- | ---- | ----------- | ------ |
| `Id` | `TKey` | The unique identifier for the entity. | Inherited from `CoreEntity<TKey>` |
| `CreatedAt` | `DateTime` | The date and time when the entity was created. | Defined in `WriteEntity<TKey>` |
| `LastModifiedAt` | `DateTime?` | The date and time when the entity was last modified. | Defined in `WriteEntity<TKey>` |

### 4. SoftDeleteEntity&lt;TKey&gt;
Inherits from `WriteEntity<TKey>` and adds support for soft deletion. Instead of physically removing records from the database, entities can be marked as deleted.

| Property | Type | Description | Source |
| -------- | ---- | ----------- | ------ |
| `Id` | `TKey` | The unique identifier for the entity. | Inherited from `CoreEntity<TKey>` |
| `CreatedAt` | `DateTime` | The date and time when the entity was created. | Inherited from `WriteEntity<TKey>` |
| `LastModifiedAt` | `DateTime?` | The date and time when the entity was last modified. | Inherited from `WriteEntity<TKey>` |
| `IsDeleted` | `bool` | A flag indicating whether the entity is marked as deleted. | Defined in `SoftDeleteEntity<TKey>` |

### 5. AuditableBaseEntity&lt;TKey&gt;
Inherits from `WriteEntity<TKey>` and adds properties to track which user created or modified the entity.

| Property | Type | Description | Source |
| -------- | ---- | ----------- | ------ |
| `Id` | `TKey` | The unique identifier for the entity. | Inherited from `CoreEntity<TKey>` |
| `CreatedAt` | `DateTime` | The date and time when the entity was created. | Inherited from `WriteEntity<TKey>` |
| `LastModifiedAt` | `DateTime?` | The date and time when the entity was last modified. | Inherited from `WriteEntity<TKey>` |
| `CreatedBy` | `string?` | The identifier of the user who created the entity. | Defined in `AuditableBaseEntity<TKey>` |
| `LastModifiedBy` | `string?` | The identifier of the user who last modified the entity. | Defined in `AuditableBaseEntity<TKey>` |

## 🚀 Usage Examples

### Example 1: Creation-Only Entity (`BaseEntity<TKey>`)
For append-only entities (such as logs or audit trails) that only require an ID and creation timestamp:

```csharp
using RA.Utilities.Data.Entities;

public class AuditLog : BaseEntity<long>
{
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}
```

### Example 2: Mutable Entity with Timestamps (`WriteEntity<TKey>`)
For standard entities requiring creation and modification timestamps:

```csharp
using System;
using RA.Utilities.Data.Entities;

public class Product : WriteEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
```

When you instantiate `Product`, it contains:
* `Id` (`Guid`)
* `CreatedAt` (`DateTime`)
* `LastModifiedAt` (`DateTime?`)
* `Name` (`string`)
* `Description` (`string?`)
* `Price` (`decimal`)

### Example 3: Auditable and Soft-Deletable Entity
If you need an entity that combines both soft deletion and user auditing, define a combined base class:

```csharp
using System;
using RA.Utilities.Data.Entities;

/// <summary>
/// Represents an entity that supports both soft deletion and user auditing.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class AuditableSoftDeleteEntity<TKey> : AuditableBaseEntity<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
```

Then inherit from your base class:

```csharp
public class Order : AuditableSoftDeleteEntity<Guid>
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid CustomerId { get; set; }
}
```

The `Order` entity includes `Id`, `CreatedAt`, `LastModifiedAt`, `CreatedBy`, `LastModifiedBy`, and `IsDeleted`.

### Using with Entity Framework Core

These base entities integrate seamlessly with EF Core. You can configure them in your `DbContext`:

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

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main [official documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/Data/Entities/).

## Feedback

If you have suggestions or find a bug, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities).
Contributions are welcome!