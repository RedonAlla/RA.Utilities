---
title: EmailValidatorExtensions
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Application.Validation.Extensions
```

`EmailValidatorExtensions` adds a reusable [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rule that validates a string property through the [`Email`](../../core/RA.Utilities.Core.ValueObjects/Email.md) value object of `RA.Utilities.Core.ValueObjects` — so request validation and the domain share one set of email rules.

## 🎯 Purpose and Design

### 1. Value-Object-Backed Rule

`MustBeValidEmail` chains onto any `string?` rule and validates the value by constructing an `Email` instance behind the scenes:

```csharp showLineNumbers
public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        // highlight-next-line
        RuleFor(x => x.Email).MustBeValidEmail();
    }
}
```

Any input the `Email` value object rejects fails the rule for that property path: `null`, empty or whitespace, addresses over 254 characters, and values that don't match the email format.

### 2. Requiredness Is Enforced

Because the `Email` constructor rejects `null`, empty, and whitespace input, this rule also fails on missing values — the value doubles as a required check.

### 3. Failure Message

The rule surfaces the `BadRequestException` message of the value object — the generic `"The request is invalid."` — rather than the specific validation detail. Chain `WithMessage` to provide a property-specific message:

```csharp showLineNumbers
RuleFor(x => x.Email)
    .MustBeValidEmail()
    .WithMessage("'{PropertyName}' must be a valid email address.");
```

## ✔️ Valid vs Invalid

| Value | Result |
|---|---|
| `"user@gmail.com"` | ✅ Valid |
| `"  User@Gmail.com  "` | ✅ Valid (normalized to `user@gmail.com`) |
| `null` | ❌ Invalid (unlike the format-only currency rule) |
| `""` / `"   "` | ❌ Invalid |
| `"not-an-email"` | ❌ Invalid |
| > 254 characters | ❌ Invalid |

## 🧠 Summary

`EmailValidatorExtensions` delegates email validation to the `Email` value object, keeping request validation and domain rules in sync. Unlike shape-only format rules, it fails on missing values and reports failures with the generic `BadRequestException` message.
