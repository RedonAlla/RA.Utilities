```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseErrorMessage` class provides a collection of predefined, constant string messages for field-level validation failures.
It lives within the `RA.Utilities.Core.Constants` package and is designed to work alongside [`BaseErrorCode`](./BaseErrorCode): the code identifies the failure programmatically, while the message explains it to humans.

While [`BaseResponseMessages`](./BaseResponseMessages) covers whole-response messaging (e.g., "The requested resource was not found."), `BaseErrorMessage` describes input-level problems: a missing email address, an invalid SSN, a currency mismatch, and so on.

Using these constants offers the same benefits as the rest of the package:

1. **Consistency**: Every service in your ecosystem reports the same failure with the same wording, giving clients a predictable experience.
2. **Readability**: `BaseErrorMessage.EmailRequired` is self-documenting, unlike a scattered string literal.
3. **Maintainability**: If a message needs rewording, you change it in one central location.
4. **Reduced Errors**: Named constants prevent typos in string literals that are difficult to spot.

These messages are used by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`) when they throw `BadRequestException` with structured `ValidationError` entries.

## Constant Values

| Constant Name        | Message                                    |
|----------------------|--------------------------------------------|
| **EmailRequired**    | "The 'Email address' field is required."   |
| **EmailNotValid**    | "'Email address' format is invalid."       |
| **CurrencyRequired** | "The 'Currency' field is required."        |
| **CurrencyLength**   | "'Currency' must be exact 3 characters."   |
| **CurrencyMismatch** | "Currency mismatch"                        |
| **PositiveMoney**    | "POSITIVE_MONEY"                           |
| **SsnRequired**      | "The 'SSN' field is required."             |
| **SsnLength**        | "'SSN' must be exact 10 characters."       |
| **SsnNotValid**      | "'SSN' format is invalid."                 |

> **Note:** There is no `EmailMaxLength` constant. The maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text (e.g., "Email address must not exceed 254 characters."). See [`BaseErrorCode.EmailMaxLength`](./BaseErrorCode) for the matching error code.

## 🚀 Usage Examples

### Throw a structured validation error

Use the message as the `ValidationError` description and the matching code from [`BaseErrorCode`](./BaseErrorCode) as its `ErrorCode`:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

public class PaymentService
{
    public void Validate(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            // highlight-next-line
            throw new BadRequestException(new ValidationError(BaseErrorMessage.CurrencyRequired)
            {
                PropertyName = "Currency",
                ErrorCode = BaseErrorCode.CurrencyRequired
            });
        }

        // ... proceed with the payment
    }
}
```

### Pair codes and messages in a problem-details response

The structured error is serialized into the HTTP 400 response body, so consumers see both the code and the human-readable message:

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "The request is invalid.",
  "result": [
    {
      "propertyName": "Email",
      "errorMessage": "The 'Email address' field is required.",
      "errorCode": "REQUIRED_EMAIL"
    }
  ]
}
```
