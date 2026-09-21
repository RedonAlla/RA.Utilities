---
title: Currency
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Core.ValueObjects
```

The `Currency` value object represents a validated currency code. Values are trimmed and upper-cased on creation and must be exactly 3 characters, as defined by ISO 4217 (e.g., `EUR`, `USD`).

## 🎯 Purpose

Currency codes appear everywhere in financial domains — prices, payments, transfers. Passing them around as raw strings invites casing inconsistencies (`eur` vs `EUR`) and length mistakes (`EU` vs `EUR`). `Currency` validates the shape of the code once, so downstream code (like [`Money`](./Money.md)) can rely on it.

## Properties

| Property  | Type     | Description                              |
|-----------|----------|------------------------------------------|
| **Value** | `string` | The normalized code, e.g. `EUR`.         |

## Validation rules

| Rule                     | ErrorCode (from `BaseErrorCode`)          | Notes                                   |
|--------------------------|-------------------------------------------|-----------------------------------------|
| Missing or whitespace    | `CurrencyRequired` (`REQUIRED_CURRENCY`)  | `PropertyName` is `Currency`            |
| Not exactly 3 characters | `CurrencyLength` (`CURRENCY_LENGTH`)      | `AttemptedValue` and `ExpectedValue` (`"EUR"`) are populated |

All failures are reported as a `BadRequestException` whose `Errors` collection contains a single `ValidationError`.

:::info Length, not membership
`Currency` validates that the code has exactly 3 characters. It does not check the value against the official ISO 4217 list, so custom or future codes pass validation by design.
:::

## 🚀 How to Use

### Create from user input

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;

// highlight-next-line
var currency = new Currency(" eur ");

currency.Value; // "EUR"
```

### Parse and TryParse

`Currency` implements `IParsable<Currency>`, which enables binding in ASP.NET Core minimal APIs:

```csharp showLineNumbers
// highlight-next-line
app.MapGet("/prices/{currency}", (Currency currency) => GetPrice(currency));

var parsed = Currency.Parse("usd", provider: null); // Currency with Value "USD"
```

### Equality and conversions

```csharp showLineNumbers
new Currency(" eur ") == new Currency("EUR"); // true — value equality

string code = new Currency("EUR"); // implicit conversion to string
```

There is deliberately no implicit conversion *from* `string`, because that would bypass validation.

### JSON serialization

`Currency` carries a built-in `System.Text.Json` converter. It serializes as a plain string and deserializes back through validation:

```csharp showLineNumbers
var json = JsonSerializer.Serialize(new Currency("EUR"));
// json == "\"EUR\""

// highlight-next-line
var currency = JsonSerializer.Deserialize<Currency>("\"usd\"");
// currency.Value == "EUR"

JsonSerializer.Deserialize<Currency>("\"EURO\""); // throws JsonException
```

### Combine with Money

```csharp showLineNumbers
// highlight-next-line
var price = new Money(19.99m, new Currency("EUR"));
```
