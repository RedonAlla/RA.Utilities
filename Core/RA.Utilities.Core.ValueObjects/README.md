# RA.Utilities.Core.ValueObjects

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.ValueObjects.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.ValueObjects/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Core.ValueObjects.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.ValueObjects/)
[![Documentation](https://img.shields.io/badge/documentation-view-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.ValueObjects/)

`RA.Utilities.Core.ValueObjects` provides strongly-typed, validated value objects like `Email`, `Currency`, `SSN`, and `Money`.
It solves the problem of passing raw, unvalidated strings and decimals around your domain, where formatting inconsistencies and invalid data can silently propagate.

By creating an instance of a value object, the input is validated and normalized *once*, at the boundary of your system. After that, an `Email` that exists is always a valid, normalized email address.
- **Type Safety**: You can't accidentally pass a currency code where an email is expected; the compiler enforces intent.
- **Validation at the Boundary**: Invalid input throws a `BadRequestException` with structured `ValidationError` details (error codes, property names, attempted values) that `RA.Utilities.Api` middleware can translate into standardized **HTTP 400** responses automatically.
- **Consistent Normalization**: Values are trimmed and case-normalized on creation, so `new Email(" User@Gmail.com ")` and `new Email("user@gmail.com")` are equal.
- **JSON Support**: Built-in `System.Text.Json` converters serialize value objects as plain strings and deserialize them back with validation.
- **Value Equality**: All value objects are records, so they compare by value, not reference.

---

## Table of Contents

- Getting started
- How It Works
- Available Value Objects
  - `Email`
  - `Currency`
  - `SSN`
  - `Money`
- JSON Serialization
- Extensibility
- Best Practices
- Additional documentation
- Contributing

---

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core.ValueObjects
```

Or through the NuGet Package Manager in Visual Studio.

---

## How It Works

This package is designed to fail fast at the edges of your system.

1. **Boundary Validation**: When user input (e.g., from a request DTO) creates a value object, the input is validated and normalized immediately. Invalid input throws a `BadRequestException`.
2. **Guaranteed Validity**: Inside your domain, an `Email`, `Currency`, or `SSN` instance that exists is always valid and normalized. No re-validation, no defensive checks.
3. **Automatic Error Responses**: In your API project (e.g., using `RA.Utilities.Api`), a global error-handling middleware catches the `BadRequestException` and translates it into a standardized **HTTP 400 Bad Request** response with structured validation details.

---

## Available Value Objects

### `Email`

A validated, normalized email address. Values are trimmed and lower-cased on creation, must not exceed 254 characters, and must match a standard email format.

**Usage:**

```csharp
public void RegisterUser(string emailInput)
{
    var email = new Email(emailInput); // throws BadRequestException if invalid

    // LocalPart == "user", Domain == "gmail.com"
    var existing = _userRepository.FindByEmail(email);
    if (existing is not null)
    {
        throw new ConflictException("User", email);
    }
    // ... registration logic
}
```

**Key members:**
- `Value`: the normalized address (e.g., `user@gmail.com`).
- `LocalPart` / `Domain`: the parts before and after the `@` sign.
- `Parse` / `TryParse`: `IParsable<Email>` support for route, query, and header binding in minimal APIs.
- Implicit conversion to `string`.

### `Currency`

A validated currency code. Values are trimmed and upper-cased on creation and must be exactly 3 characters, as defined by ISO 4217 (e.g., `EUR`, `USD`).

**Usage:**

```csharp
public Money GetPrice(string currencyCode)
{
    var currency = new Currency(currencyCode); // throws BadRequestException if not exactly 3 characters
    return new Money(19.99m, currency);
}
```

**Key members:**
- `Value`: the normalized code (e.g., `EUR`).
- `Parse` / `TryParse`: `IParsable<Currency>` support for minimal API binding.
- Implicit conversion to `string`.

### `SSN`

A validated social security number. Values are trimmed and upper-cased on creation, must not exceed 10 characters, and must match the format `^[A-Z][0-9]{8}[A-Z]$` (a letter, eight digits, a letter).

**Usage:**

```csharp
public void UpdateEmployee(string ssnInput)
{
    var ssn = new SSN(ssnInput); // throws BadRequestException if invalid
    _employeeRepository.Update(ssn, update);
}
```

**Key members:**
- `Value`: the normalized SSN (e.g., `J01234567R`).
- `Parse` / `TryParse`: `IParsable<SSN>` support for minimal API binding.
- Implicit conversion to `string`.

### `Money`

A monetary amount paired with a `Currency`. Arithmetic is guarded so you can't combine amounts in different currencies.

**Usage:**

```csharp
var price = Money.PositiveMoney(19.99m, new Currency("EUR")); // throws BadRequestException if negative
var shipping = Money.PositiveMoney(4.99m, new Currency("EUR"));

var total = price.Add(shipping); // 24.98 EUR
// total.Add(new Money(5m, new Currency("USD"))) would throw BadRequestException (CurrencyMismatch)
```

**Key members:**
- `Amount` (`decimal`) and `Currency`.
- `Money.PositiveMoney(amount, currency)`: factory that rejects negative amounts.
- `Add(other)`: adds two amounts; throws `BadRequestException` if the currencies differ.

---

## JSON Serialization

Every value object carries a built-in `System.Text.Json` converter (`ValueObjectJsonConverter<T>`). Value objects serialize as plain strings and deserialize back through validation:

```csharp
string json = JsonSerializer.Serialize(new Email("User@Gmail.com"));
// json == "\"user@gmail.com\""

Email? email = JsonSerializer.Deserialize<Email>("\" User@Gmail.com \"");
// email.Value == "user@gmail.com" (normalized)

// Invalid input throws JsonException, as expected from a JsonConverter
JsonSerializer.Deserialize<Currency>("\"EURO\""); // throws JsonException
```

---

## Extensibility

The shared behavior is defined by `IValueObject<TSelf>` (a normalized `Value` string plus `IParsable<TSelf>`). To create your own string-based value object with the same JSON converter support, implement the interface and reuse the generic converter:

```csharp
[JsonConverter(typeof(ValueObjectJsonConverter<Sku>))]
public sealed partial record Sku : IValueObject<Sku>
{
    public string Value { get; }

    public Sku(string? value)
    {
        // validate and normalize...
        Value = value.Trim().ToUpperInvariant();
    }

    public static Sku Parse(string s, IFormatProvider? provider) => new(s);

    public static bool TryParse(string? s, IFormatProvider? provider, out Sku result) { /* ... */ }
}
```

---

## Best Practices

1. **Validate at the Boundary, Trust Inside the Domain**:
   Create value objects as early as possible (e.g., when mapping request DTOs). After that, rely on the type system: an `Email` that exists is valid.

2. **Prefer Value Objects over Primitive Strings in Signatures**:
   Passing `Email` instead of `string` makes invalid states unrepresentable and eliminates duplicated validation logic across your application.

3. **Let the API Layer Handle Failures**:
   Don't catch `BadRequestException` in your domain or application layers for flow control. Let it bubble up to the `RA.Utilities.Api` middleware, which produces a standardized, structured HTTP 400 response.

4. **Use `Money` for Amounts with Currency**:
   Never store a monetary amount as a bare `decimal`. `Money` prevents cross-currency arithmetic mistakes at compile time and runtime.

---

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.ValueObjects/).

---

## Contributing

Contributions are welcome! If you have a suggestion for a new value object or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.
