---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Data.Entities
```

# IEntity

The `IEntity` interface serves as a **marker interface** for all entities in the `RA.Utilities.Data.Entities` package.
It defines no methods or properties — its sole purpose is to provide a common parent type for every entity base class, starting with [`CoreEntity<TKey>`](./CoreEntity.md).

## What is a Marker Interface?

A marker interface is an interface without members.
It "tags" a family of related types so they can be discovered, grouped, and constrained as a single category, without forcing them into a specific base class.

Here is its full definition:

```csharp
namespace RA.Utilities.Data.Entities;

/// <summary>
/// Marker interface implemented by all entities (see <see cref="CoreEntity{TKey}"/>).
/// It defines no members; its purpose is to let generic abstractions, such as the repository interfaces, constrain their type parameters to entities.
/// </summary>
public interface IEntity;
```

## Why Is It Used Here?

### 1. Decoupled Generic Constraints

Generic abstractions — most notably the repository contracts in [`RA.Utilities.Data.Abstractions`](../Abstractions/index.mdx) — constrain their type parameter with `where T : IEntity` instead of requiring a concrete base class:

```csharp
public interface IRepositoryBase<T> : IReadRepositoryBase<T>, IWriteRepositoryBase<T> where T : IEntity;
```

This means you are not locked into the `CoreEntity<TKey>` hierarchy.
A custom entity can implement `IEntity` directly and still flow through the standard repository abstractions.

### 2. Architectural Consistency

The marker establishes a clear, foundational contract that identifies a type as an entity.
It improves readability across the codebase and gives infrastructure code a single, stable type to scan or filter for.

## Role in the Entity Hierarchy

Every base class in this package is an `IEntity`, with [`CoreEntity<TKey>`](./CoreEntity.md) providing the typed identifier:

```
IEntity (marker — no members)
└── CoreEntity<TKey>
    ├── BaseEntity<TKey>
    └── WriteEntity<TKey>
        ├── SoftDeleteEntity<TKey>
        └── AuditableBaseEntity<TKey>
```

## Summary

`IEntity` adds no behavior itself, but it decouples the ecosystem's generic data contracts from any specific base class, keeping entity hierarchies flexible while remaining strongly typed.
