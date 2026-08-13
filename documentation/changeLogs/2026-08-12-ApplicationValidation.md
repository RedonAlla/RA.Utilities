---
title: RA.Utilities.Application.Validation
authors: [RedonAlla]
---

# Release Notes

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
