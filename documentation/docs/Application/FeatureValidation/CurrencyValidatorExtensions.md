---
title: CurrencyValidatorExtensions
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Application.Validation.Extensions
```

`CurrencyValidatorExtensions` adds a reusable [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rule that checks whether a string property looks like an **ISO 4217** currency code — exactly 3 uppercase letters (`USD`, `EUR`, `GBP`).

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

## ✔️ Valid vs Invalid

| Value | Result |
|---|---|
| `"USD"`, `"EUR"`, `"GBP"`, `"XAU"` | ✅ Valid |
| `null` | ✅ Passes (add `NotNull()` to require) |
| `"usd"` (lowercase) | ❌ Invalid |
| `"US"` / `"USDD"` (wrong length) | ❌ Invalid |
| `"U5D"`, `"U$D"`, `"US D"` (non-letters) | ❌ Invalid |
| `""` (empty) | ❌ Invalid |

## 🧠 Summary

`CurrencyValidatorExtensions` centralizes currency-code validation so every part of an application validates currencies in the same way. It uses modern C# features (source-generated regex), follows [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) conventions, and leaves requiredness (`NotNull` / `NotEmpty`) to the caller.
