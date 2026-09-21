# Release Notes for RA.Utilities.Core.ValueObjects

## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-21%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.ValueObjects/10.0.0)

This is the first stable release of `RA.Utilities.Core.ValueObjects`, a package of strongly-typed, validated value objects with built-in JSON support. Instances are validated and normalized once, at the boundary of your system; an instance that exists is always valid. Invalid input throws a `BadRequestException` with structured `ValidationError` details (error codes from `BaseErrorCode`, messages from `BaseErrorMessage`) that `RA.Utilities.Api` middleware can translate into standardized HTTP 400 responses.

### ✨ New Features

*   **`Email`**: A validated email address, trimmed and lower-cased on creation, with a 254-character maximum and format validation. Exposes `Value`, `LocalPart`, and `Domain`, plus `Parse`/`TryParse` (`IParsable<Email>`) for minimal API binding, implicit conversion to `string`, and value equality.
*   **`Currency`**: A validated currency code per ISO 4217 (exactly 3 characters), trimmed and upper-cased on creation. Exposes `Value`, `Parse`/`TryParse` (`IParsable<Currency>`), implicit conversion to `string`, and value equality.
*   **`SSN`**: A validated social security number, trimmed and upper-cased on creation, with a 10-character maximum and format validation (`^[A-Z][0-9]{8}[A-Z]$`). Exposes `Value`, `Parse`/`TryParse` (`IParsable<SSN>`), implicit conversion to `string`, and value equality.
*   **`Money`**: A monetary amount (`decimal`) paired with a `Currency`. Provides the `PositiveMoney(amount, currency)` factory, which rejects negative amounts, and `Add(other)`, which throws a `BadRequestException` when the currencies differ.
*   **`ValueObjectJsonConverter<T>`**: A generic `System.Text.Json` converter shared by all value objects. Value objects serialize as plain strings and deserialize back through validation; invalid JSON payloads throw `JsonException`.
*   **`IValueObject<TSelf>`**: An interface combining a normalized `Value` string with `IParsable<TSelf>`, allowing consumers to write extension methods and generic algorithms over any value object in this package.

### 📝 Notes

All value objects are records and compare by value. Validation errors are reported through `BadRequestException` with `ValidationError` entries carrying `PropertyName`, `ErrorCode`, `AttemptedValue`, and `ExpectedValue`.

---
