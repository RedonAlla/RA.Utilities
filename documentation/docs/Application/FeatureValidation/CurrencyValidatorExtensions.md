---
title: CurrencyValidatorExtensions
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Application.Validation.Extensions
```

`CurrencyValidatorExtensions` adds two reusable [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rules for currency codes:

* **`MustMatchesCurrencyFormat()`** — checks whether a string property looks like an **ISO 4217** currency code: exactly 3 uppercase letters (`USD`, `EUR`, `GBP`).
* **`MustBeValidCurrency()`** — validates the property through the [`Currency`](../../core/RA.Utilities.Core.ValueObjects/Currency.md) value object of `RA.Utilities.Core.ValueObjects`, so request validation and the domain share one set of rules.

Both rules are declared as C# 14 extension members on `IRuleBuilder<T, string>`, so they chain naturally onto string rules.

## 🎯 Purpose and Design

### 1. FluentValidation Extension

`MustMatchesCurrencyFormat` chains onto any string rule, keeping currency checks declarative and consistent across validators:

```csharp showLineNumbers
public class MyRequestValidator : AbstractValidator<MyRequest>
{
    public MyRequestValidator()
    {
        RuleFor(x => x.Currency).MustMatchesCurrencyFormat();
    }
}
```

### 2. Validation Logic

The private `IsValid` method checks the value against a compiled regex for the ISO 4217 format:

* **Null values pass the rule** — requiredness is a separate concern; chain `NotNull()` / `NotEmpty()` when the currency is required.
* **Empty strings fail** — an empty string is not a currency code.
* **Only the format is checked** — this rule does not verify that the code exists in the official ISO 4217 list.

### 3. High-Performance Regex

The class uses the modern C# `[GeneratedRegex("^[A-Z]{3}$")]` feature to produce a highly optimized, source-generated regular expression at compile time — more performant than a `new Regex(...)` created at runtime.

### Clear Error Message

Failures carry a user-friendly message — `"Currency must be 3 uppercase letters (A-Z)."` — which you can override with FluentValidation's `WithMessage`:

```csharp showLineNumbers
RuleFor(x => x.Currency)
    .MustMatchesCurrencyFormat()
    .WithMessage("'{PropertyName}' must be a valid ISO 4217 currency code.");
```

## 🏛️ Value-Object-Backed Rule: `MustBeValidCurrency()`

`MustBeValidCurrency` validates the property by constructing a [`Currency`](../../core/RA.Utilities.Core.ValueObjects/Currency.md) instance behind the scenes:

```csharp showLineNumbers
public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        // highlight-next-line
        RuleFor(x => x.Currency).MustBeValidCurrency();
    }
}
```

Any input the `Currency` value object rejects fails the rule for that property path. Note that the value object checks the normalized 3-**character** length, not the letters themselves, so inputs like `"U5D"` or `"123"` pass this rule even though they fail `MustMatchesCurrencyFormat()`. Two behaviors distinguish it from the format rule:

* **`null`, empty, and whitespace fail the rule** — the `Currency` constructor rejects missing values, so the rule doubles as a required check. (`MustMatchesCurrencyFormat()` lets `null` pass and leaves requiredness to `NotNull()` / `NotEmpty()`.)
* **The failure message is the value object's generic `BadRequestException` message** — `"The request is invalid."` — not the rule-specific format message. Chain `WithMessage` for a property-specific message:

```csharp showLineNumbers
RuleFor(x => x.Currency)
    .MustBeValidCurrency()
    .WithMessage("'{PropertyName}' must be a valid ISO 4217 currency code.");
```

## ✔️ Valid vs Invalid

`MustMatchesCurrencyFormat()`:

| Value | Result |
|---|---|
| `"USD"`, `"EUR"`, `"GBP"`, `"XAU"` | ✅ Valid |
| `null` | ✅ Passes (add `NotNull()` to require) |
| `"usd"` (lowercase) | ❌ Invalid |
| `"US"` / `"USDD"` (wrong length) | ❌ Invalid |
| `"U5D"`, `"U$D"`, `"US D"` (non-letters) | ❌ Invalid |
| `""` (empty) | ❌ Invalid |

`MustBeValidCurrency()`:

| Value | Result |
|---|---|
| `"USD"`, `"usd"`, `" eur "` | ✅ Valid (trimmed and upper-cased, e.g. `"usd"` → `USD`) |
| `null` | ❌ Invalid (the value object requires a value) |
| `""` / `"   "` | ❌ Invalid |
| `"US"` / `"USDD"` | ❌ Invalid |
| `"U5D"` / `"123"` | ✅ Valid (only the 3-character length is checked) |

## 🧠 Summary

`CurrencyValidatorExtensions` centralizes currency-code validation so every part of an application validates currencies in the same way. `MustMatchesCurrencyFormat()` is a shape-only check with a friendly message and source-generated regex; `MustBeValidCurrency()` delegates to the `Currency` value object, fails on missing values, and reports failures with the generic `BadRequestException` message.
