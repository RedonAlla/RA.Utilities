---
title: Money
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Core.ValueObjects
```

The `Money` value object pairs a monetary `Amount` (`decimal`) with a [`Currency`](./Currency.md). It guards arithmetic so amounts in different currencies can never be combined accidentally.

## 🎯 Purpose

A bare `decimal` cannot express what it represents: 100 of *what*? Combining amounts across currencies silently is one of the most expensive bugs in financial software. `Money` makes the currency part of the type and enforces the "same currency" rule at runtime, while `decimal` preserves the precision that `double` and `float` cannot.

## Properties

| Property     | Type        | Description                                  |
|--------------|-------------|----------------------------------------------|
| **Amount**   | `decimal`   | The numeric monetary amount.                 |
| **Currency** | `Currency`  | The currency of the amount.                  |

## Members

| Member                                   | Description                                                       |
|------------------------------------------|-------------------------------------------------------------------|
| `Money(decimal amount, Currency currency)` | Creates a `Money` instance. Validation-free; see `PositiveMoney`. |
| `PositiveMoney(decimal, Currency)`       | Factory that throws `BadRequestException` when the amount is negative. |
| `Add(Money other)`                       | Returns a new `Money` with the sum; throws `BadRequestException` when currencies differ. |
| `ToString()`                             | Returns `"<Amount> <Currency>"`, e.g. `24.98 EUR`.               |

## Validation rules

| Rule                          | ErrorCode (from `BaseErrorCode`)   | Notes                                                  |
|-------------------------------|------------------------------------|--------------------------------------------------------|
| Negative amount in `PositiveMoney` | `PositiveMoney` (`POSITIVE_MONEY`) | `PropertyName` is `Amount`, `AttemptedValue` is the amount |
| Different currencies in `Add` | `CurrencyMismatch` (`CURRENCY_MISMATCH`) | `AttemptedValue` is the other currency, `ExpectedValue` is the current one |

All failures are reported as a `BadRequestException` whose `Errors` collection contains a single `ValidationError`.

## 🚀 How to Use

### Create positive amounts

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;

// highlight-next-line
var price = Money.PositiveMoney(19.99m, new Currency("EUR"));
var shipping = Money.PositiveMoney(4.99m, new Currency("EUR"));

Money.PositiveMoney(-1m, new Currency("EUR")); // throws BadRequestException
```

### Add amounts safely

```csharp showLineNumbers
// highlight-next-line
var total = price.Add(shipping); // Money { Amount = 24.98, Currency = EUR }

total.ToString(); // "24.98 EUR"

// highlight-next-line
total.Add(new Money(5m, new Currency("USD"))); // throws BadRequestException (CurrencyMismatch)
```

`Add` returns a new instance; `Money` is immutable, like all value objects in this package.

### Equality

```csharp showLineNumbers
new Money(10m, new Currency("EUR")) == new Money(10m, new Currency("EUR")); // true
new Money(10m, new Currency("EUR")) == new Money(10m, new Currency("USD")); // false
```

:::note JSON serialization
Unlike the string-based value objects, `Money` has no built-in JSON converter and serializes as a regular object with `Amount` and `Currency` properties. If you need a custom representation (e.g. `"24.98 EUR"` or a `{ amount, currency }` contract), write a dedicated `JsonConverter<Money>` in your application.
:::
