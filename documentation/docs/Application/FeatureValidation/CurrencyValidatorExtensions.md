---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Application.Validation.Extensions
```

Based on the source code you've provided, the `CurrencyValidatorExtensions` class is a static helper designed to enhance the [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) library with a specific, reusable validation rule for currency codes.

## 🎯 Purpose and Design

The primary goal of this class is to provide a clean, declarative, and efficient way to ensure that a string property represents a valid **ISO 4217** currency code.

Here's a breakdown of its key components and how they achieve this:

#### 1. **FluentValidation Extension**:
The class is `static` and contains the `MustMatchesCurrencyFormat` extension method. This design allows developers to chain this custom validation rule onto any string property within their [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) validator classes, like so:

```csharp
public class MyRequestValidator : AbstractValidator<MyRequest>
{
    public MyRequestValidator()
    {
        RuleFor(x => x.Currency).MustMatchesCurrencyFormat();
    }
}
```

#### 2. Validation Logic:
The core logic is encapsulated in the private `IsValid` method.
It checks two things:

* That the value is not `null`.
* That the value matches a specific regular expression.

#### 3. High-Performance Regex:
The class uses a modern C# feature, `[GeneratedRegex("^[A-Z]{3}$")]`, to create a highly optimized, source-generated regular expression at compile time.
This regex specifically checks if the string consists of exactly three uppercase letters (e.g., "USD", "EUR", "GBP"), which is the standard format for ISO 4217 currency codes.
This is more performant than creating a `new Regex(...)` at runtime.

#### Clear Error Message:
If the validation fails, it provides a user-friendly error message: `"Currency must be 3 uppercase letters (A-Z)."`.
This makes it easy to understand why a given value was rejected.

## 🧠 Summary
In short, `CurrencyValidatorExtensions` is a well-designed utility that follows best practices to create a reusable and performant validation component.
It centralizes the logic for currency code validation, preventing code duplication and ensuring that all parts of an application validate currencies in a consistent manner.

The code is clean, efficient, and well-documented.
There are no immediate improvements to suggest as it effectively uses modern C# features and adheres to the principles of the [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) library.