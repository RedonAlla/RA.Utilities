---
title: IbanValidatorExtensions
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Application.Validation.Extensions
```

`IbanValidatorExtensions` adds a reusable [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rule that validates a string property through the [`IBAN`](../../core/RA.Utilities.Core.ValueObjects/IBAN.md) value object of `RA.Utilities.Core.ValueObjects` — so request validation and the domain share one set of IBAN rules.

## 🎯 Purpose and Design

### 1. Value-object-backed rule

`MustBeValidIban` chains onto any string rule and validates the value by constructing an `IBAN` instance behind the scenes:

```csharp showLineNumbers
public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentValidator()
    {
        // highlight-next-line
        RuleFor(x => x.Iban).MustBeValidIban();
    }
}
```

Any input the `IBAN` value object rejects fails the rule for that property path: `null`, empty or whitespace, values outside the 13–34 character structure, malformed country/check digits, and values that fail the ISO 7064 mod-97 check.

The rule is declared as a C# 14 extension member on `IRuleBuilder<T, string>`; the calling syntax is unchanged. It works for both synchronous `Validate` and asynchronous `ValidateAsync`.

### 2. Normalization

Valid input is normalized exactly as the value object normalizes it: trimmed, stripped of grouping separators (spaces, apostrophes, backslashes, hyphens, and non-breaking spaces), and upper-cased. These all pass:

```
GB82WEST12345698765432
gb82west12345698765432
GB82 WEST 1234 5698 7654 32
GB82-WEST-1234-5698-7654-32
```

### 3. Requiredness is enforced

Because the `IBAN` constructor rejects `null`, empty, and whitespace input, this rule also fails on missing values — the value doubles as a required check. Use a condition (`.When(...)`) if the IBAN is genuinely optional.

### 4. Failure message

The rule surfaces the `BadRequestException` message of the value object — the generic `"The request is invalid."` — rather than the specific validation detail. The attempted value is not echoed back. Chain `WithMessage` to provide a property-specific message:

```csharp showLineNumbers
RuleFor(x => x.Iban)
    .MustBeValidIban()
    .WithMessage("'{PropertyName}' must be a valid IBAN.");
```

## ✔️ Valid vs Invalid

| Value | Result |
|---|---|
| `"GB82WEST12345698765432"` | ✅ Valid (compact) |
| `"gb82 west 1234 5698 7654 32"` | ✅ Valid (normalized) |
| `"DE89 3704 0044 0532 0130 00"` | ✅ Valid |
| `"FR14 2004 1010 0505 0001 3M02 606"` | ✅ Valid (alphanumeric BBAN) |
| `"NO93 8601 1117 947"` | ✅ Valid (shortest registry entry) |
| `"MT84 MALT 0110 0001 2345 MTLC AST0 01S"` | ✅ Valid (longest registry entry) |
| `null` / `""` / `"   "` | ❌ Invalid (required check) |
| `"GB82"` (too short) | ❌ Invalid |
| `"GB82WEST123456987654321234567890123"` (too long) | ❌ Invalid |
| `"G182WEST12345698765432"` (country code not letters) | ❌ Invalid |
| `"GB8AWEST12345698765432"` (check digits not digits) | ❌ Invalid |
| `"GB82_WEST12345698765432"` (underscore separator) | ❌ Invalid |
| `"GB81WEST12345698765432"` (wrong check digits) | ❌ Invalid |

## What the rule does not check

- **Per-country length** — the value object validates the 13–34 character structural range and the mod-97 checksum, not the length each country's registry requires. A structurally valid, checksum-passing IBAN that is the wrong length for its country can pass. Validate country lengths separately and report `BaseErrorCode.IbanLength` when your domain needs that.
- **Country registry membership** — the country code must be two letters, but it is not checked against the SWIFT IBAN registry.
- **Bank existence** — a valid IBAN does not prove that the account exists or is reachable.

## 🧠 Summary

`IbanValidatorExtensions` delegates IBAN validation to the `IBAN` value object, keeping request validation and domain rules in sync. It normalizes formatting, enforces the structure and ISO 7064 checksum, fails on missing values, and reports failures with the generic `BadRequestException` message. For country-specific length rules, validate separately — see [`RA.Utilities.Core.Constants`](../../core/RA.Utilities.Core.Constants/BaseErrorCode).
