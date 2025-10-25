---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Data.Abstractions
```

The `IReadRepositoryBase<T>` interface is a specialized contract that defines a set of read-only operations for a generic repository.
Its primary purpose is to provide a standardized way to query data from a data source without allowing any modifications.

This design is a direct application of the **Command Query Separation (CQS)** principle, which states that methods should either be commands that change the state of the system or queries that return data, but not both.

## 🔑 Key Features and Benefits

#### 1. Enforces Read-Only Access:
By implementing this interface, you can create repository implementations that are guaranteed not to have methods for adding, updating, or deleting data.
This is extremely useful for parts of your application that should only be able to retrieve information, such as reporting services or data-display components.

#### 2. Provides Standard Query Methods:
It comes with a set of powerful and flexible methods for common querying scenarios:

  * `GetByIdAsync<TId>`: Retrieves a single entity by its unique identifier.
  * `ListAsync`: A versatile method to retrieve a list of entities with support for filtering, sorting, pagination (`skip`/`take`), and eager loading of related data (includeProperties).

#### 3. Promotes a Clean Architecture:
It helps create a clear separation of concerns in your data access layer.
You can have services that depend only on `IReadRepositoryBase<T>` to ensure they cannot accidentally modify data.

---

## ⚙️ Methods

### `GetByIdAsync<TId>`

#### Purpose

The `GetByIdAsync` method is designed for one simple but very common task: **retrieving a single entity from the database using its unique primary key**.
It's the most direct way to fetch a specific record when you know its ID.
#### Parameters

| Parameter         | Type                | Description                                                              |
| :---------------- | :------------------ | :----------------------------------------------------------------------- |
| `id`              | `TId`               | The unique identifier of the entity to retrieve.                         |
| `cancellationToken` | `CancellationToken` | (Optional) A token to observe while waiting for the task to complete. ß |

#### Example

Here is how you might use `GetByIdAsync` in a service to find a specific product.

```csharp
public class ProductService
{
    private readonly IRepositoryBase<Product> _productRepository;

    public ProductService(IRepositoryBase<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductDetails(Guid productId)
    {
        // Use GetByIdAsync to fetch a single product by its Guid ID.
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            // Handle the case where the product was not found.
            Console.WriteLine($"Product with ID {productId} not found.");
            return null;
        }

        return product;
    }
}
```

### `ListAsync`

#### Purpose

The `ListAsync` method is a highly flexible and powerful method for **querying a list of entities**.
It allows you to filter, sort, and paginate your data directly at the database level, which is far more efficient than fetching all records into memory and then processing them.

#### Parameters
It accepts several optional parameters to build a precise query:

| Parameter           | Type                                                | Description                                                                 |
| :------------------ | :-------------------------------------------------- | :-------------------------------------------------------------------------- |
| `filter`            | `Expression<Func<T, bool>>?`                        | (Optional) A LINQ expression to filter the results (e.g., `p => p.IsActive`). |
| `orderBy`           | `Func<IQueryable<T>, IOrderedQueryable<T>>?`        | (Optional) A function to sort the results (e.g., `q => q.OrderBy(p => p.Name)`). |
| `skip`              | `int?`                                              | (Optional) The number of records to skip for pagination.                    |
| `take`              | `int?`                                              | (Optional) The number of records to return for pagination.                  |
| `cancellationToken`   | `CancellationToken`                                 | (Optional) A token to observe while waiting for the task to complete.       |
| `includeProperties` | `params Expression<Func<T, object>>[]`              | (Optional) An array of navigation properties to eagerly load.               |

#### Example

Here’s an example of using `ListAsync` to find the first 10 "active" products that cost more than $50, sorted by price, and also loading their `Category` information.


```csharp showLineNumbers
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Data.Entities;

public class ProductService
{
    private readonly IRepositoryBase<Product> _productRepository;

    public ProductService(IRepositoryBase<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<Product>> GetTopPricedActiveProducts(int pageNumber, int pageSize)
    {
        // Calculate the number of records to skip for pagination.
        int recordsToSkip = (pageNumber - 1) * pageSize;

        // Use ListAsync to build a complex query.
        var products = await _productRepository.ListAsync(
            // 1. Filter: Only get products that are not soft-deleted and cost more than 50.
            filter: p => !p.IsDeleted && p.Price > 50.00m,
            
            // 2. Order By: Sort the results by price in descending order.
            orderBy: q => q.OrderByDescending(p => p.Price),
            
            // 3. Paginate: Skip the records from previous pages.
            skip: recordsToSkip,
            
            // 4. Paginate: Take only the number of records for the current page.
            take: pageSize
        );

        return products;
    }
}
```