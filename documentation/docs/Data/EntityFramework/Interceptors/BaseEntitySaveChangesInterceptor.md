---
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Interceptors
```

The primary purpose of the `BaseEntitySaveChangesInterceptor` is to automatically set timestamp properties on your entities whenever they are created or modified.
It "intercepts" the `SaveChanges` operation right before the changes are sent to the database and injects its own logic.

This solves a common problem: ensuring that every record has accurate `CreatedAt` and `LastModifiedAt` timestamps without requiring developers to set these values manually every time.

## ⚙️ How It Works
#### 1. Inheritance:
The class inherits from `SaveChangesInterceptor`, a base class provided by EF Core for this exact purpose.

#### 2. Overriding Methods:
It overrides the `SavingChanges` and `SavingChangesAsync` methods.
These methods are automatically called by Entity Framework Core just before it writes any pending changes (inserts, updates, deletes) to the database.

#### 3. Detecting Changes:
Inside these methods, it calls the UpdateEntities helper method.
This method uses the `DbContext.ChangeTracker` to find all tracked entities that inherit from `BaseEntity`.

#### 4. Applying Timestamps:
It then checks the state of each entity:

  * If an entity's state is `Added`, it means it's a new record.
  The interceptor sets its `CreatedAt` property to the current UTC time.
  * If an entity's state is `Modified`, it means an existing record has been changed.
  The interceptor sets its `LastModifiedAt` property to the current UTC time.

Here is the key logic from the `UpdateEntities` method that accomplishes this:

```csharp
foreach (EntityEntry<BaseEntity> entry in context.ChangeTracker.Entries<BaseEntity>())
{
    if (entry.State is EntityState.Added)
    {
        entry.Entity.CreatedAt = DateTime.UtcNow;
    }

    if (entry.State is EntityState.Modified)
    {
        entry.Entity.LastModifiedAt = DateTime.UtcNow;
    }
}
```

## 💭 Why Is This Useful?
  * **Consistency**: It guarantees that auditing fields are always populated correctly.
  * **Reduces Boilerplate**: Developers don't have to write repetitive code to set these timestamps in their business logic.
  * **Centralized Logic**: The auditing logic is in one central, reusable place, making the system easier to maintain and understand.
  * **Decoupling**: It separates the concern of auditing from the main business logic of your application.

To make this work, the interceptor needs to be registered with the `DbContext`.
This registration tells Entity Framework Core to use this interceptor during its `SaveChanges` pipeline.

```csharp
// In PersistenceDependencyInjection.cs
services.AddScoped<BaseEntitySaveChangesInterceptor>();

services.AddDbContext<TodoDbContext>((provider, options) => options
    .UseInMemoryDatabase(connectionString)
    .AddInterceptors(provider.GetRequiredService<BaseEntitySaveChangesInterceptor>()
));
```