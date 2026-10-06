---
title: Domain Layer
sidebar_position: 2
---

# Domain Layer

The Domain layer lives in `src/Core/RaTemplate.Domain` and is the innermost tier of the architecture. It holds the enterprise rules and core business concepts, completely isolated from presentation, persistence, and external frameworks. Because it has no outward dependencies, the model can evolve independently of databases, HTTP, or UI concerns.

:::info[Template state:]
the scaffold ships an intentionally empty `Entities/` folder plus the `Constants/` building blocks below.
You add your own entities, value objects, and constants — the conventions and base types are already in place and enforced by the architecture tests.
:::

## Project Dependencies

The Domain project references only two shared libraries from `RA.Utilities` — no other project in the solution:

| Package | Purpose |
| :--- | :--- |
| `RA.Utilities.Data.Entities` | Base entity types (`CoreEntity<TKey>`, `BaseEntity<TKey>`, `WriteEntity<TKey>`, `IEntity`) |
| `RA.Utilities.Core.ValueObjects` | Reusable, validated value objects (`Email`, `Currency`, `Money`, `SSN`) |

This deliberate minimal coupling keeps the core pure: everything else in the solution depends *on* Domain, never the reverse.

## Entities

Entities are objects that maintain a persistent identity across their lifetime while enforcing structural invariants. All entities derive from one of the shared base types in `RA.Utilities.Data.Entities`:

| Base Type | Provides | Use When |
| :--- | :--- | :--- |
| `CoreEntity<TKey>` | A typed `Id` (identity only) | You manage timestamps yourself or don't need them |
| `BaseEntity<TKey>` | `Id` + `CreatedAt` | You need a creation timestamp |
| `WriteEntity<TKey>` | `Id` + `CreatedAt` + `LastModifiedAt` | You need creation and modification tracking |

```csharp
namespace RaTemplate.Domain.Entities;

public sealed class Customer : WriteEntity<Guid>
{
    public required string Name { get; init; }
    public required Email Email { get; init; }
}
```

`TKey` is the identity type (`Guid`, `int`, `long`, or a custom key). Persistence infrastructure fills the audit fields automatically.

### Entity conventions (enforced)

The `EntitiesTests` in `tests/RaTemplate.ArchitectureTests/` guarantee three rules at build/test time:

1. Every type in the `RaTemplate.Domain.Entities` namespace **inherits from `CoreEntity<>`**.
2. Every entity **is `sealed`** — no ad-hoc inheritance hierarchies.
3. **Only** types in the `RaTemplate.Domain.Entities` namespace inherit from `CoreEntity<>` — nothing else in Domain masquerades as an entity.

## Value Objects

Value objects are immutable types defined solely by their properties, with no distinct identity. The `RA.Utilities.Core.ValueObjects` package provides validated, normalized building blocks:

| Value Object | Represents | Normalization / Validation |
| :--- | :--- | :--- |
| `Email` | An email address | Trimmed, lower-cased, RFC 5321 length, format-checked |
| `Currency` | An ISO 4217 currency code | Trimmed, upper-cased, exactly 3 characters |
| `Money` | An amount paired with a `Currency` | Factory-validated (e.g. non-negative), currency-safe arithmetic |
| `SSN` | A social security number | Length and format validated |

Each of these implements `IValueObject<TSelf>` (a normalized `Value` plus `IParsable<TSelf>`), so an instance that exists is always valid. They expose:

- **Constructors / `Parse` / `TryParse`** that validate on creation — `TryParse` supports minimal-API route/query/header binding and produces a 400 on failure.
- **A `[JsonConverter]`** for clean serialization to and from their normalized string form.
- **Implicit conversion to `string`** (lossless), with no reverse conversion, so validation cannot be bypassed.

```csharp
// Always valid or it throws a BadRequestException carrying an error code
Email email = new("  User@Gmail.com ");   // Value == "user@gmail.com"
Money total = Money.PositiveMoney(100m, new Currency("eur"));
```

`Money` also offers currency-aware operations such as `Add`, which throws if the two operands use different currencies.

## Domain Constants

Two constant holders ship with the template under `Constants/`:

- **`EntitiesConstraints`** — a static class for entity property constraints such as maximum string lengths. Add constants here and reference them from entities, EF configuration, and validators so limits are defined once.

```csharp
public static class EntitiesConstraints
{
    public const int CustomerNameMaxLength = 200;
}
  ```

- **`ErrorCodes`** — derives from `RA.Utilities.Core.Constants.BaseErrorCode`, inheriting the shared error codes (`EmailRequired`, `EmailNotValid`, `CurrencyMismatch`, `PositiveMoney`, and more) and serving as the place to add domain-specific codes.

```csharp
public class ErrorCodes : BaseErrorCode
{
    public const string CustomerNotFound = "CUSTOMER_NOT_FOUND";
}
```

These codes surface through validation errors and problem details so clients get stable, machine-readable identifiers.

## Assembly Reference

`AssemblyReference.cs` is a sealed marker type. It carries no logic — its sole purpose is to let the composition root and other layers locate the Domain assembly by `typeof(AssemblyReference).Assembly`, which is how validators, handlers, and mappings are discovered via reflection.

## Cross-Layer Rules

Complete isolation from outer modules keeps the Domain pure:

- Domain references **only** the two `RA.Utilities` packages — never Application, Infrastructure, or Presentation.
- Outer layers depend inward on Domain, consuming its entities, value objects, and constants.
- The `DependencyTests` in the architecture test project enforce this direction, so an accidental outward reference fails the build.
