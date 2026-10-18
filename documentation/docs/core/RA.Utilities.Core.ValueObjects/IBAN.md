---
title: IBAN
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Core.ValueObjects
```

The `IBAN` value object represents a validated, normalized International Bank Account Number. Values are trimmed, stripped of grouping separators (spaces, apostrophes, backslashes, and hyphens), upper-cased on creation, checked against the ISO 13616 structural pattern (13–34 alphanumeric characters), and verified against the ISO 7064 mod-97 check digit algorithm.

## 🎯 Purpose

Banking and payment systems process account identifiers from diverse sources — manual user entries, paper invoices, web forms, and third-party APIs. Raw IBAN strings frequently contain varied formatting (such as grouped paper notation `GB82 WEST 1234 5698 7654 32`), lower-case characters, or transposed digits.

Passing unvalidated strings risks payment routing errors or bank rejections. The `IBAN` value object guarantees that any instance in your domain is:
- **Canonical**: all separators removed and normalized to uppercase.
- **Structurally valid**: conforms to the 2-letter country code, 2 check digits, and 9–30 alphanumeric BBAN structure.
- **Mathematically verified**: check digits satisfy the ISO 7064 mod-97 algorithm.

## Properties

| Property | Type | Description |
|---|---|---|
| **Value** | `string` | The normalized, compact IBAN (e.g. `GB82WEST12345698765432`). |
| **CountryCode** | `string` | The 2-letter ISO 3166-1 alpha-2 country code extracted from the first two characters (e.g. `GB`). |

## Validation rules

| Rule | ErrorCode (from `BaseErrorCode`) | Notes |
|---|---|---|
| Missing or whitespace | `IbanRequired` (`REQUIRED_IBAN`) | `PropertyName` is `IBAN` |
| Invalid format / structure | `IbanNotValid` (`NOT_VALID_IBAN`) | `AttemptedValue` contains the normalized input; `ExpectedValue` describes the pattern (`"2a2n[9-30c] in 2 to 7 groups of 3 to 5 characters"`) |
| Check digits mismatch | `IbanCheckDigits` (`IBAN_CHECK_DIGITS`) | `AttemptedValue` is normalized input; `ExpectedValue` contains the computed check digits |

All failures are reported as a `BadRequestException` whose `Errors` collection contains a single `ValidationError`.

:::info Country-specific lengths vs. structural validation
The `IBAN` value object validates the international standard structural envelope (13 to 34 characters) and the ISO 7064 mod-97 check digits. It intentionally does not maintain an internal per-country length table, ensuring that newly chartered countries or updated specifications pass without requiring library updates.

If your application needs to enforce country-specific lengths (for example, Germany's 22 characters or France's 27 characters), perform that registry check in your application layer using `BaseErrorCode.IbanLength` (`IBAN_LENGTH`).
:::

## 🚀 How to Use

### Create from user input

`IBAN` automatically strips whitespace and grouping characters (spaces, apostrophes, backslashes, hyphens) and converts the value to upper case:

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;

// highlight-next-line
var iban = new IBAN("gb82 west 1234-5698-7654-32");

Console.WriteLine(iban.Value);       // "GB82WEST12345698765432"
Console.WriteLine(iban.CountryCode); // "GB"
```

Invalid input throws a `BadRequestException` immediately:

```csharp showLineNumbers
// highlight-next-line
new IBAN("GB00WEST12345698765432"); // throws BadRequestException (IbanCheckDigits)
```

### Parse and TryParse

`IBAN` implements `IParsable<IBAN>`, enabling minimal API binding from route, query, and header values:

```csharp showLineNumbers
// highlight-next-line
app.MapGet("/accounts/{iban}", (IBAN iban) => GetAccountByIban(iban));

var parsed = IBAN.Parse("GB82WEST12345698765432", provider: null);

// highlight-next-line
if (IBAN.TryParse("GB82 WEST 1234 5698 7654 32", provider: null, out var validIban))
{
    Console.WriteLine(validIban.CountryCode); // "GB"
}
```

### Equality and conversions

Because `IBAN` is a record, comparisons are based on the normalized value:

```csharp showLineNumbers
var compact = new IBAN("GB82WEST12345698765432");
var formatted = new IBAN("gb82 west 1234 5698 7654 32");

compact == formatted; // true — value equality

// Implicit conversion to string (lossless)
string raw = compact;

// ToString returns the normalized compact value
compact.ToString();   // "GB82WEST12345698765432"
```

There is deliberately no implicit conversion *from* `string`, because that would bypass validation.

### JSON serialization

`IBAN` carries the `[JsonConverter(typeof(ValueObjectJsonConverter<IBAN>))]` attribute. It serializes as a plain string and deserializes back through constructor validation:

```csharp showLineNumbers
var json = JsonSerializer.Serialize(new IBAN("GB82WEST12345698765432"));
// json == "\"GB82WEST12345698765432\""

// highlight-next-line
var iban = JsonSerializer.Deserialize<IBAN>("\" gb82 west 1234 5698 7654 32 \"");
// iban.Value == "GB82WEST12345698765432"

JsonSerializer.Deserialize<IBAN>("\"INVALID_IBAN\""); // throws JsonException
```
