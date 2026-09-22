```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseErrorCode` class is a constants class within the `RA.Utilities.Core.Constants` package.
Its primary purpose is to provide a centralized, single source of truth for the machine-readable error codes returned when field-level validation fails.

Where [`BaseResponseCode`](./BaseResponseCode) covers transport-level concerns (which HTTP status to return), `BaseErrorCode` describes *why* a request was rejected: a missing email address, an invalid SSN, a currency mismatch, and so on. Each code is a short, stable, uppercase string (e.g., `REQUIRED_EMAIL`, `INVALID_EMAIL`) that API consumers can branch on programmatically — unlike human-readable messages, which may be reworded over time.

The codes are grouped around the value objects shared across the RA.Utilities ecosystem:

1. **Email** — required, maximum length, and format checks
2. **Currency** — required, exact length, and cross-field mismatch checks
3. **Money** — positive-amount checks
4. **SSN** — required, exact length, and format checks

`BaseErrorCode` is designed to be paired with [`BaseErrorMessage`](./BaseErrorMessage), which holds the matching human-readable text. Together they populate the structured `ValidationError` entries thrown by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`), which the `RA.Utilities.Api` middleware translates into standardized HTTP 400 responses.

## Constant Values

| Constant Name      | Value              | Triggered When                                            |
|--------------------|--------------------|-----------------------------------------------------------|
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

> **Note:** There is no `BaseErrorMessage.EmailMaxLength` counterpart. The maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text (e.g., "Email address must not exceed 254 characters."), so only the error code is a constant.

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
