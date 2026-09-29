---
title: IDbContext
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Data.Abstractions
```

The `IDbContext` interface is the abstraction for your database context (like Entity Framework's `DbContext`).
It defines the contract through which consumers persist changes — `SaveChangesAsync` — without depending on a concrete context class or a specific ORM.

```csharp showLineNumbers
using System.Threading;
using System.Threading.Tasks;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Represents a generic interface for a database context.
/// </summary>
public interface IDbContext
{
    /// <summary>
    ///     Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous save operation.
    ///     The task result contains the number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

## Members

| Member | Description |
|--------|-------------|
| `SaveChangesAsync(CancellationToken)` | Asynchronously saves all changes made in this context to the database. Returns a `Task<int>` whose result is the number of state entries written to the database. |

## 🧠 Here's a breakdown of its purpose:

#### 1. Decoupling from a Specific ORM:
By having your repositories and services depend on `IDbContext` instead of a concrete class like [`Microsoft.EntityFrameworkCore.DbContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-9.0), your data access logic is no longer tied directly to Entity Framework.
This adheres to the **Dependency Inversion Principle**, where high-level modules (your repositories) should not depend on low-level modules (the ORM implementation), but both should depend on abstractions.

#### 2. Enhancing Testability:
When your code depends on an interface, you can easily create mock or fake implementations of `IDbContext` in your unit tests.
This allows you to test your logic in complete isolation, without needing to connect to a real database, making your tests faster and more reliable.

#### 3. Enforcing a Clean Architecture:
`IDbContext` acts as a boundary.
Your application and domain layers can reference the `RA.Utilities.Data.Abstractions` package to use `IDbContext`, while the concrete `DbContext` implementation resides in your infrastructure layer.
This prevents your core business logic from having a direct dependency on infrastructure concerns.

## ⚙️ How It's Used in Practice

Implement `IDbContext` on your concrete context. With Entity Framework Core this requires no extra code, because `DbContext` already exposes a matching `SaveChangesAsync(CancellationToken)`:

```csharp showLineNumbers
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Data.Abstractions;

namespace YourApp.Persistence;

public class AppDbContext : DbContext, IDbContext
{
    // DbContext.SaveChangesAsync(CancellationToken) already satisfies
    // the IDbContext contract — nothing else to implement.
}
```

Consumers can then persist changes through the abstraction, keeping infrastructure details out of application code:

```csharp showLineNumbers
public async Task<int> CheckoutAsync(IDbContext context, Order order,
    CancellationToken cancellationToken)
{
    // ... stage changes through repositories ...

    // highlight-next-line
    return await context.SaveChangesAsync(cancellationToken);
}
```

## ⚠️ Breaking Change in v10.0.4
Prior to `10.0.4`, `IDbContext` was an empty marker interface.
If your codebase implements `IDbContext`, you must now also implement `SaveChangesAsync(CancellationToken)` (with Entity Framework Core, declaring the interface is enough — `DbContext` already provides the method).
See the [changelog](/changelogs) for full details and migration steps.

## 🧠 Summary
In summary, `IDbContext` provides a minimal, ORM-agnostic contract for persisting changes, creating a clean, maintainable, and testable data access layer by abstracting away the specific details of the database context implementation.
