---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Data.Abstractions
```

The `IWriteRepositoryBase<T>` interface is a specialized contract that defines a set of **write-only** operations for a generic repository.
Its primary purpose is to provide a standardized way to create, update, and delete entities in a data source.

This design is a direct application of the **Command Query Separation (CQS)** principle, which states that methods should either be *commands* that change the state of the system (like the ones in this interface) or *queries* that return data, but not both.

## 🔑 Key Features and Benefits

#### 1. Enforces Write-Only Access
By depending on this interface, a service can be explicitly designed to only modify data, preventing it from performing read operations. This creates a clear and secure separation of concerns.

#### 2. Provides Standard Mutation Methods
It defines a comprehensive set of methods for all common data modification tasks:
*   `AddAsync` / `AddRangeAsync`: For creating one or more entities.
*   `UpdateAsync` / `UpdateRangeAsync`: For modifying existing entities.
*   `DeleteAsync` / `DeleteRangeAsync`: For removing entities.

#### 3. Promotes a Clean Architecture
It allows you to build services that are clearly defined as "command handlers," whose sole responsibility is to change the state of the application, leading to a more maintainable and understandable architecture.


## ⚙️ Methods

| Method               | Return Type | Description                               |
| :------------------- | :---------- | :---------------------------------------- |
| `AddAsync`           | `Task<T>`   | Adds a single new entity.                 |
| `AddRangeAsync`      | `Task<int>` | Adds a collection of new entities.        |
| `UpdateAsync`        | `Task<T>`   | Updates a single existing entity.         |
| `UpdateRangeAsync`   | `Task<int>` | Updates a collection of existing entities.|
| `DeleteAsync`        | `Task`      | Deletes a single entity by its ID.        |
| `DeleteRangeAsync`   | `Task<int>` | Deletes a collection of entities by their IDs. |

### `AddAsync`
Marks a single new entity to be added to the database.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `entity` | `T`	| The entity to add. |
| `cancellationToken` | `CancellationToken` |	(Optional) A token to observe while waiting for the task to complete. |

### `AddRangeAsync`
Marks a collection of new entities to be added.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `entities` | `IEnumerable<T>`	| The collection of entities to add. |
| `cancellationToken` | `CancellationToken`	| (Optional) A token to observe while waiting for the task to complete. |

### `UpdateAsync`
Marks an existing entity as modified.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `entity` | `T`	| The entity to update.	|
| `cancellationToken`	| CancellationToken	|	(Optional) A token to observe while waiting for the task to complete.

### `UpdateRangeAsync`
Marks a collection of existing entities as modified.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `entities` | `IEnumerable<T>`	| The collection of entities to update.	|
| `cancellationToken` | `CancellationToken	|	(Optional) A token to observe while waiting for the task to complete.	|

### `DeleteAsync`
Marks an existing entity for deletion.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `id` | `TId`	| The unique identifier of the entity to delete.	|
| `cancellationToken` | `CancellationToken`	| (Optional) A token to observe while waiting for the task to complete.	|

### `DeleteRangeAsync`
Marks a collection of existing entities for deletion.

| Parameter	| Type	| Description |
| --------	| ----	| ----------- |
| `ids` | `IEnumerable<TId>`	| A collection of unique identifiers of the entities to delete.	|
| `cancellationToken` | `CancellationToken`	| (Optional) A token to observe while waiting for the task to complete.	|