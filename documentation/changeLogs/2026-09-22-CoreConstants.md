---
title: RA.Utilities.Core.Constants
authors: [RedonAlla]
---

## Version 10.0.3
![Date Badge](https://img.shields.io/badge/Publish-22%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/10.0.3)

This release introduces a shared vocabulary for field-level validation failures: `BaseErrorCode` for machine-readable error codes and `BaseErrorMessage` for the matching human-readable messages.

<!-- truncate -->

### ✨ New Features

*   **`BaseErrorCode`**: New constants class providing stable, machine-readable error codes for validation failures around email (`REQUIRED_EMAIL`, `EMAIL_MAX_LENGTH`, `INVALID_EMAIL`), currency (`REQUIRED_CURRENCY`, `CURRENCY_LENGTH`, `CURRENCY_MISMATCH`), money (`POSITIVE_MONEY`), and SSN (`REQUIRED_SSN`, `SSN_LENGTH`, `NOT_VALID_SSN`). API consumers can branch on these codes programmatically without parsing message text.
*   **`BaseErrorMessage`**: New constants class with the matching human-readable messages (e.g., `The 'Email address' field is required.`), ensuring consistent wording for validation failures across all services.
*   **Value object integration**: These constants back the structured `ValidationError` entries thrown by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`) and are translated into standardized HTTP 400 responses by the `RA.Utilities.Api` middleware.

### 📝 Notes

*   `BaseErrorCode.EmailMaxLength` intentionally has no `BaseErrorMessage` counterpart — the maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text.
