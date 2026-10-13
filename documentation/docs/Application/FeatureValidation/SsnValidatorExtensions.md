---
title: SsnValidatorExtensions
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Application.Validation.Extensions
```

`SsnValidatorExtensions` adds a reusable [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rule that validates a string property through the [`SSN`](../../core/RA.Utilities.Core.ValueObjects/SSN.md) value object of `RA.Utilities.Core.ValueObjects` — so request validation and the domain share one set of SSN rules.

## 🎯 Purpose and Design

### 1. Value-Object-Backed Rule

`MustBeValidSsn` chains onto any `string?` rule and validates the value by constructing an `SSN` instance behind the scenes:

```csharp showLineNumbers
public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        // highlight-next-line
        RuleFor(x => x.Ssn).MustBeValidSsn();
    }
}
```

Any input the `SSN` value object rejects fails the rule for that property path: `null`, empty or whitespace, values over 10 characters, and values that don't match the `^[A-Z][0-9]{8}[A-Z]$` format (letter + 8 digits + letter).

### 2. Requiredness Is Enforced

Because the `SSN` constructor rejects `null`, empty, and whitespace input, this rule also fails on missing values — the value doubles as a required check.

### 3. Failure Message

The rule surfaces the `BadRequestException` message of the value object — the generic `"The request is invalid."` — rather than the specific validation detail. Chain `WithMessage` to provide a property-specific message:

```csharp showLineNumbers
RuleFor(x => x.Ssn)
    .MustBeValidSsn()
    .WithMessage("'{PropertyName}' must be a valid SSN.");
```

## ✔️ Valid vs Invalid

| Value | Result |
|---|---|
| `"J01234567R"` | ✅ Valid |
| `" j01234567r "` | ✅ Valid (trimmed and upper-cased to `J01234567R`) |
| `null` | ❌ Invalid (unlike shape-only format rules) |
| `""` / `"   "` | ❌ Invalid |
| `"0123456789"` (no letters) | ❌ Invalid |
| `"J0123456R"` (7 digits) | ❌ Invalid |

## 🧠 Summary

`SsnValidatorExtensions` delegates SSN validation to the `SSN` value object, keeping request validation and domain rules in sync. Unlike shape-only format rules, it fails on missing values and reports failures with the generic `BadRequestException` message.
