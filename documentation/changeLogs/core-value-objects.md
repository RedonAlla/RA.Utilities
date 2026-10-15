---
title: RA.Utilities.Core.ValueObjects
authors: [RedonAlla]
date: 2026-10-11
---

# Release Notes

## Version 10.1.0

![Date Badge](https://img.shields.io/badge/Publish-11%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.ValueObjects/10.1.0)

Feature release: adds `Money.Subtract` for currency-safe subtraction.

<!-- truncate -->

### ✨ New Features

*   **`Money.Subtract(other)`**: returns a new `Money` with the difference of the two amounts. Mirrors `Add(other)`: throws a `BadRequestException` with a `CurrencyMismatch` validation error (`AttemptedValue` = the other currency, `ExpectedValue` = the instance currency) when the currencies differ.

```csharp
var price = Money.PositiveMoney(19.99m, new Currency("EUR"));
var discount = Money.PositiveMoney(4.00m, new Currency("EUR"));

var net = price.Subtract(discount); // 15.99 EUR
// price.Subtract(new Money(4m, new Currency("USD"))) throws BadRequestException (CurrencyMismatch)
```

### 📝 Notes

*   `Money` is immutable: `Add` and `Subtract` return new instances and never mutate the operands.

## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-21%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.ValueObjects/10.0.0)

This is the first stable release of `RA.Utilities.Core.ValueObjects`, introducing strongly-typed, validated value objects for .NET with built-in `System.Text.Json` support, structured validation errors via `BadRequestException`, and minimal API binding through `IParsable<T>`.

<!-- truncate -->

### ✨ New Features

*   **`Email`**: A validated, normalized email value object. Trims and lowercases input, enforces a 254-character maximum, and exposes `LocalPart` and `Domain` properties.
*   **`Currency`**: A validated currency value object. Trims and uppercases input and enforces the 3-character ISO 4217 length.
*   **`SSN`**: A validated social-security-number value object. Trims, uppercases, and enforces the 10-character `^[A-Z][0-9]{8}[A-Z]$` format.
*   **`Money`**: A monetary amount pairing a `decimal` with a `Currency`, with a `PositiveMoney` factory and currency-safe `Add`.
*   **`IValueObject<TSelf>`**: A shared contract extending `IParsable<TSelf>` with a normalized `Value` property, making it easy to build your own validated value objects.
*   **`ValueObjectJsonConverter<T>`**: A generic `System.Text.Json` converter that serializes value objects as plain strings and validates them on deserialization, applied out of the box to `Email`, `Currency`, and `SSN`.
*   **Structured validation errors**: Every validation failure throws a `BadRequestException` carrying `ValidationError` entries with property name, message, error code, attempted value, and expected value — ready for the RA.Utilities API error pipeline.
*   **Minimal API binding**: `Parse`/`TryParse` implementations enable route, query, and header binding in ASP.NET Core minimal APIs, with failed bindings producing automatic 400 responses.

### 📝 Notes

There are no breaking changes — `10.0.0` is the initial release. See the [Getting Started guide](/RA.Utilities/nuget-packages/core/RA.Utilities.Core.ValueObjects/) to adopt the package.
