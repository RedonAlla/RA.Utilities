---
title: RA.Utilities.Application.Validation
authors: [RedonAlla]
date: 2026-10-14
---

# Release Notes

## Version 10.1.0

![Date Badge](https://img.shields.io/badge/Publish-11%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Application.Validation/10.1.0)

Feature release: adds FluentValidation rules that validate through the value objects of `RA.Utilities.Core.ValueObjects` — so request validation and the domain share one set of rules — and adds `CancellationToken` support to `ValidationUtilities.ValidateAsync`.

<!-- truncate -->

### ✨ New Features

* **`MustBeValidCurrency()`**, **`MustBeValidEmail()`** and **`MustBeValidSsn()`** (in `RA.Utilities.Application.Validation.Extensions`): `Custom` rules for `string?` properties that construct the matching value object behind the scenes. Any input the value object rejects fails the rule for the property path.
* Because the value object constructors reject `null`, empty and whitespace input, these rules also fail on missing values — unlike `MustMatchesCurrencyFormat()`, where `null` passes and requiredness is left to `NotNull()` / `NotEmpty()`.
* The failure message is the generic `BadRequestException` message ("The request is invalid."), not the value object's specific message; chain `WithMessage(...)` for a property-specific message.

```csharp
public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).MustBeValidEmail();
        RuleFor(x => x.Currency).MustBeValidCurrency();
        RuleFor(x => x.Ssn).MustBeValidSsn();
    }
}
```

### ⚠️ Breaking Changes

* **`ValidationUtilities.ValidateAsync` now requires a `CancellationToken`**: the new third parameter has no default, so existing two-argument calls no longer compile — pass `CancellationToken.None` or forward the request's token. The token is passed to every validator, enabling cooperative cancellation. **Migration**: see the [migration guide](/nuget-packages/Application/FeatureValidation/migration-guides).

### 📝 Improvements

* **Faster validation orchestration**: `ValidateAsync` returns an empty array immediately when no validators are registered, awaits a single validator directly instead of going through `Task.WhenAll`, no longer re-materializes validator collections that are already arrays (the standard DI case), and uses `ConfigureAwait(false)` throughout.

### 📦 Dependencies

* New dependency on `RA.Utilities.Core.ValueObjects` (10.1.0) — the new rules construct `Currency`, `Email` and `SSN` instances to validate input. `RA.Utilities.Core.Exceptions` remains a direct dependency for `ValidationUtilities.CreateValidationErrorResult`.

## Version 10.0.1

![Date Badge](https://img.shields.io/badge/Publish-12%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Application.Validation/10.0.1)

Bug-fix release: corrects duplicated validation failures from parallel validators, fixes null handling in `MustMatchesCurrencyFormat`, and adopts the renamed `ValidationError` type from `RA.Utilities.Core.Exceptions` 10.0.4.

<!-- truncate -->

### 🐛 Bug Fixes

* **Fixed duplicate and cross-contaminated validation failures**: `ValidateAsync` now creates a fresh `ValidationContext` per validator instead of sharing one across `Task.WhenAll`. In 10.0.0 the shared context held a single mutable failure list, so every validator's result contained all failures (one failing rule appeared once per registered validator), with a data race on top when validators were genuinely async.
* **Fixed `MustMatchesCurrencyFormat` rejecting null**: null values now pass the format rule. FluentValidation 12 runs `Must` on null, so in 10.0.0 an optional currency field failed with the format message. Chain `NotNull()` / `NotEmpty()` for required currencies.

### ⚠️ Breaking Changes

* **`ValidationErrors` → `ValidationError`**: `CreateValidationErrorResult` now maps failures to the renamed `ValidationError` type (from `RA.Utilities.Core.Exceptions` 10.0.4), whose constructor requires `errorMessage`. **Migration**: rename references to `ValidationErrors` and pass the message to the constructor — see the [migration guide](/nuget-packages/Application/FeatureValidation/migration-guides).

### 📝 Improvements

* **Added test coverage** — 24 tests covering `ValidationUtilities` and `CurrencyValidatorExtensions` (parallel aggregation, failure mapping, currency format edge cases).

## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-12%20January%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Application.Validation/10.0.0)

- Initial release of the `RA.Utilities.Application.Validation` package.
- Provides utility validators using [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) for incoming requests.

<!-- truncate -->
