---
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Extensions
```
The `DependencyInjectionExtensions` class in the `RA.Utilities.Data.EntityFramework` project serves as a convenience layer to simplify the registration of the generic repository patterns in an ASP.NET Core application's dependency injection (DI) container.

Its primary goal is to **reduce boilerplate** and **promote convention** by providing simple, one-line extension methods for [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection).

Instead of developers needing to remember and write the full generic type registration like this:

```csharp
// Manual, more verbose registration
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
```
They can use the much cleaner and more readable extension method provided by this class:
```csharp
// Simplified registration using the extension method
builder.Services.AddRepositoryBase();
```

This approach has several key benefits:

* **Simplicity & Readability**: It makes the `Program.cs` file cleaner and the intent clearer.
* **Reduced Errors**: It abstracts away the `typeof()` syntax, reducing the chance of typos or incorrect registrations.
* **Encapsulation**: It encapsulates the library's registration logic, providing a clean public API for consumers of the NuGet package.

## ✨ Available Extension Methods
The class provides three distinct methods to support different architectural needs, such as **Command Query Separation (CQS)**:

1. **`AddReadRepositoryBase()`**:
Registers the generic, read-only repository ([`IReadRepositoryBase<>`](../../Abstractions/IReadRepositoryBase.md) and [`ReadRepositoryBase<>`](../../EntityFramework/ReadRepositoryBase.md)).
This is ideal for services that only need to query data.
2. **`AddWriteRepositoryBase()`: Registers the generic, write-only repository ([`IWriteRepositoryBase<>`](../../Abstractions/IWriteRepositoryBase.md) and [`WriteRepositoryBase<>`](../../EntityFramework/WriteRepositoryBase.md)). This is for services that only modify data.
3. **`AddRepositoryBase()`: Registers the full-featured repository ([`IRepositoryBase<>`](../../Abstractions/IRepositoryBase.md) and [`RepositoryBase<>`](../../EntityFramework/RepositoryBase.md)) that handles both read and write operations.

### `AddReadRepositoryBase`
This method registers the generic **read-only repository**.
It is ideal for services that only need to query data, enforcing the ***Command Query Separation (CQS)*** principle.

#### Parameters

| Parameter |	Type	| Description |
| --------- | ----- | ----------- |
| **services** | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) |	The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the service registration to. This is the `this` parameter for the extension method. |

#### Example
Here is how you would register and use the read-only repository.

```csharp
// In your Program.cs or Startup.cs
// highlight-next-line
using RA.Utilities.Data.EntityFramework.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ... other service registrations

// Register the generic read-only repository base.
// highlight-next-line
builder.Services.AddReadRepositoryBase();

// ...

var app = builder.Build();
```

Once registered, you can inject [`IReadRepositoryBase<>`](../../Abstractions/IReadRepositoryBase.md) into any service that needs to query data for a specific entity `T`.

```csharp showLineNumbers
// In a service that only needs to read product data
public class ProductReportService
{
    // highlight-next-line
    private readonly IReadRepositoryBase<Product> _productReadRepository;

    // The DI container injects the correct repository instance.
    public ProductReportService(IReadRepositoryBase<Product> productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync()
    {
        // This repository only exposes query methods.
        // highlight-next-line
        return await _productReadRepository.ListAsync(filter: p => p.IsActive);
    }
}
```

### `AddWriteRepositoryBase`
This method registers the generic **write-only** repository.
It should be used in services that are responsible for creating, updating, or deleting data.

#### Parameters

| Parameter |	Type	| Description |
| --------- | ----- | ----------- |
| **services** | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) |	The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the service registration to. This is the `this` parameter for the extension method. |

#### Example
Here is how you would register and use the write-only repository.

```csharp
// In your Program.cs or Startup.cs
// highlight-next-line
using RA.Utilities.Data.EntityFramework.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ... other service registrations

// Register the generic write-only repository base.
// highlight-next-line
builder.Services.AddWriteRepositoryBase();

// ...

var app = builder.Build();
```

You can then inject [`IWriteRepositoryBase<>`](../../Abstractions/IWriteRepositoryBase.md) into services that modify data.

```csharp showLineNumbers
// In a service that only needs to modify product data
public class ProductCommandService
{
    private readonly IWriteRepositoryBase<Product> _productWriteRepository;

    public ProductCommandService(
      // highlight-next-line
      IWriteRepositoryBase<Product> productWriteRepository)
    {
        _productWriteRepository = productWriteRepository;
    }

    public async Task CreateProductAsync(string name, decimal price)
    {
        var newProduct = new Product { Name = name, Price = price };
        
        // This repository only exposes write methods.
        // highlight-start
        await _productWriteRepository.AddAsync(newProduct);
        await _productWriteRepository.SaveChangesAsync();
        // highlight-end
    }
}
```

### `AddRepositoryBase`
This method registers the full-featured generic repository that provides both **read and write** (CRUD) operations.
It's a convenient, all-in-one solution for services that need to both query and modify data for an entity.

#### Parameters

| Parameter |	Type	| Description |
| --------- | ----- | ----------- |
| **services** | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) |	The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the service registration to. This is the `this` parameter for the extension method. |

#### Example
Here is how you would register and use the full repository.


```csharp
// In your Program.cs or Startup.cs
// highlight-next-line
using RA.Utilities.Data.EntityFramework.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ... other service registrations

// Register the generic full-featured repository base.
// highlight-next-line
builder.Services.AddRepositoryBase();

// ...

var app = builder.Build();
```
This allows you to inject [`IRepositoryBase<T>`](../../Abstractions/IReadRepositoryBase.md) for full data manipulation capabilities.

```csharp showLineNumbers
// In a service that handles all operations for a product
public class ProductManagementService
{
    // highlight-next-line
    private readonly IRepositoryBase<Product> _productRepository;

    public ProductManagementService(
        // highlight-next-line
        IRepositoryBase<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        // Use the read capabilities
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task UpdateProductPriceAsync(Guid id, decimal newPrice)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product != null)
        {
            product.Price = newPrice;
            // Use the write capabilities

            // highlight-start
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
            // highlight-end
        }
    }
}
```
