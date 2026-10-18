```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseErrorCode` class provides a centralized, single source of truth for the machine-readable error codes returned when field-level validation fails.

Where [`BaseResponseCode`](./BaseResponseCode) covers transport-level concerns (which HTTP status to return), `BaseErrorCode` describes *why* a request was rejected: a missing email address, an invalid SSN, a failed IBAN check digit, and so on. Each code is a short, stable, uppercase string (e.g., `REQUIRED_EMAIL`, `INVALID_EMAIL`) that API consumers can branch on programmatically — unlike human-readable messages, which may be reworded over time.

The codes are grouped around the value objects shared across the RA.Utilities ecosystem:

1. **Email** — required, maximum length, and format checks
2. **Currency** — required, exact length, and cross-field mismatch checks
3. **Money** — positive-amount checks
4. **SSN** — required, exact length, and format checks
5. **IBAN** — required, country-specific length, structure, and ISO 7064 mod-97 check-digit checks

`BaseErrorCode` is designed to be paired with [`BaseErrorMessage`](./BaseErrorMessage), which holds the matching human-readable text. Together they populate the structured `ValidationError` entries thrown by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`, `IBAN`), which the `RA.Utilities.Api` middleware translates into standardized HTTP 400 responses.

> **Read-only, not `const`:** Since v10.0.4 these fields are `static readonly`, not compile-time constants. Read them directly at runtime, but do not use them where C# requires a constant expression — attribute arguments, default parameter values, `case` labels, or `const` declarations. See the [migration guide](./migration-guides.mdx) for replacements.

## Values

| Field Name      | Value              | Triggered When                                            |
|-----------------|--------------------|-----------------------------------------------------------|
| **EmailRequired**  | `REQUIRED_EMAIL`   | The 'Email address' field is missing.                     |
| **EmailMaxLength** | `EMAIL_MAX_LENGTH` | The 'Email address' field exceeds the maximum length.     |
| **EmailNotValid**  | `INVALID_EMAIL`    | The 'Email address' format is invalid.                    |
| **CurrencyRequired** | `REQUIRED_CURRENCY` | The 'Currency' field is missing.                       |
| **CurrencyLength** | `CURRENCY_LENGTH`  | The 'Currency' field is not exactly 3 characters.         |
| **CurrencyMismatch** | `CURRENCY_MISMATCH` | Two currency values that must match are different.    |
| **PositiveMoney**  | `POSITIVE_MONEY`   | The 'Amount' value is negative (less than 0).             |
| **SsnRequired**    | `REQUIRED_SSN`     | The 'SSN' field is missing.                                |
| **SsnLength**      | `SSN_LENGTH`       | The 'SSN' field is not exactly 10 characters.              |
| **SsnNotValid**    | `NOT_VALID_SSN`    | The 'SSN' format is invalid.                               |
| **IbanRequired**   | `REQUIRED_IBAN`    | The 'IBAN' field is missing.                               |
| **IbanLength**     | `IBAN_LENGTH`      | The 'IBAN' total length does not match the expected length for its country. |
| **IbanNotValid**   | `NOT_VALID_IBAN`   | The 'IBAN' format or structure is invalid.                 |
| **IbanCheckDigits** | `IBAN_CHECK_DIGITS` | The 'IBAN' check digits fail the ISO 7064 mod-97 validation. |

> **Note:** There is no `BaseErrorMessage.EmailMaxLength` counterpart. The maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text (e.g., "Email address must not exceed 254 characters."), so only the error code has a reusable field.

> **IBAN note:** The `IBAN` value object performs **structural** validation (a 13–34 character pattern plus the mod-97 check-digit test), so a malformed or wrongly sized value is reported as `IbanNotValid`, not `IbanLength`. `IbanLength` is provided for consumers that validate the expected length for a specific country. See the [IBAN troubleshooting entry](./troubleshooting.mdx#when-should-i-use-iban_length-instead-of-not_valid_iban).

## 🚀 Usage Examples

### Throw a structured validation error

Pair the machine-readable code with its human-readable message when rejecting invalid input. This is exactly how the value objects in `RA.Utilities.Core.ValueObjects` report failures:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

public class RegistrationService
{
    public void Register(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            // The message explains the failure to humans...
            // highlight-next-line
            throw new BadRequestException(new ValidationError(BaseErrorMessage.EmailRequired)
            {
                PropertyName = "Email",
                // ...while the code identifies it for machines
                // highlight-next-line
                ErrorCode = BaseErrorCode.EmailRequired
            });
        }

        // ... proceed with registration
    }
}
```

### Branch on error codes client-side

Because the codes are stable strings, client applications can react to specific failures without parsing message text:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;

foreach (var error in problemResponse.Errors)
{
    // highlight-next-line
    if (error.ErrorCode == BaseErrorCode.EmailNotValid)
    {
        emailField.ShowError(error.ErrorMessage);
    }
}
```

### Report an IBAN check-digit failure

IBAN validation has four codes so clients can distinguish a missing value, a country-length mismatch, a structurally invalid value, and a failed check digit. The `IBAN` value object itself pairs the code with the offending value and the expected check digits:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

// highlight-next-line
throw new BadRequestException(new ValidationError(BaseErrorMessage.IbanCheckDigits)
{
    PropertyName = "IBAN",
    // highlight-next-line
    ErrorCode = BaseErrorCode.IbanCheckDigits,
    AttemptedValue = normalizedIban,
    ExpectedValue = expectedCheckDigits
});
```

Use `BaseErrorCode.IbanRequired` for a missing value, `BaseErrorCode.IbanNotValid` for a value that does not match the IBAN structure, and `BaseErrorCode.IbanLength` when you validate a country-specific length yourself. See [`BaseErrorMessage`](./BaseErrorMessage) for the matching messages.
