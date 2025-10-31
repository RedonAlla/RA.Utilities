<p align="center">
  <img src="https://raw.githubusercontent.com/RedonAlla/RA.Utilities/main/Assets/Images/entity.svg" alt="RA.Utilities.Data.Entities Logo" width="128">
</p>

# RA.Utilities.Data.Entities

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Data.Entities?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Data.Entities.svg)](https://www.nuget.org/packages/RA.Utilities.Data.Entities/)

This package provides a set of core abstractions and base classes for data entities within the RA.Utilities ecosystem. It helps solve the problem of boilerplate and inconsistency in data models by providing common interfaces like `IEntity<T>` and base classes with standard properties like `Id`, `CreatedDate`, and `UpdatedDate`.

The primary goal is to promote consistency and reduce repetitive code when creating data entities for use with an ORM like Entity Framework Core.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Data.Entities
```

Or through the NuGet Package Manager in Visual Studio.

## Usage

To use the package, simply have your entity classes inherit from one of the provided base classes. This automatically gives your entities a primary key and auditing fields.

### `BaseEntity<T>`

This is the most common base class. It provides an `Id` property of a specified type `T`, along with `CreatedDate` and `UpdatedDate` for auditing.

Here is an example of a `Product` entity inheriting from `BaseEntity<int>`:

```csharp
using RA.Utilities.Data.Entities;

public class Product : BaseEntity<int>
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

### `BaseEntity`

This is a non-generic version that provides a `Guid` as the primary key type.

```csharp
using RA.Utilities.Data.Entities;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Foreign key
    public Guid CustomerId { get; set; }
}
```

This `Order` class will have an `Id` property of type `Guid`.

### Using with Entity Framework Core

These base entities work seamlessly with EF Core. You can configure the properties in your `DbContext`.

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

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