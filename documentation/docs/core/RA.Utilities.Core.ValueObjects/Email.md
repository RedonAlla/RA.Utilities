---
title: Email
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Core.ValueObjects
```

The `Email` value object represents a validated, normalized email address. Values are trimmed and lower-cased on creation, must not exceed 254 characters, and must match a standard email format.

## 🎯 Purpose

Email addresses are a classic source of inconsistencies: mixed casing, surrounding whitespace, and malformed input that only fails later when a message bounces. `Email` centralizes the validation rules in one place. Create it once from user input — if construction succeeds, the address is valid and normalized forever after.

## Properties

| Property     | Type     | Description                                              |
|--------------|----------|----------------------------------------------------------|
| **Value**    | `string` | The normalized address, e.g. `user@gmail.com`.           |
| **LocalPart**| `string` | The part before the `@` sign, e.g. `user`.               |
| **Domain**   | `string` | The part after the `@` sign, e.g. `gmail.com`.           |

## Validation rules

| Rule                     | ErrorCode (from `BaseErrorCode`) | Notes                                        |
|--------------------------|----------------------------------|----------------------------------------------|
| Missing or whitespace    | `EmailRequired` (`REQUIRED_EMAIL`) | `PropertyName` is `Email`                  |
| Longer than 254 chars    | `EmailMaxLength` (`EMAIL_MAX_LENGTH`) | `AttemptedValue` contains the input       |
| Invalid format           | `EmailNotValid` (`INVALID_EMAIL`) | format: `name@example.com` (`ExpectedValue`) |

All failures are reported as a `BadRequestException` whose `Errors` collection contains a single `ValidationError` with the error code, property name, message, and attempted value.

## 🚀 How to Use

### Create from user input

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;

// highlight-next-line
var email = new Email("  User@Gmail.com  ");

email.Value;      // "user@gmail.com"
email.LocalPart;  // "user"
email.Domain;     // "gmail.com"
```

### Parse and TryParse

`Email` implements `IParsable<Email>`, which enables binding in ASP.NET Core minimal APIs and explicit parsing:

```csharp showLineNumbers
var email = Email.Parse("User@Gmail.com", provider: null); // throws BadRequestException if invalid

// highlight-next-line
if (Email.TryParse("maybe-an-email", provider: null, out var result))
{
    // use result
}
```

### Equality and conversions

```csharp showLineNumbers
new Email(" User@Gmail.com ") == new Email("user@gmail.com"); // true — value equality

string address = new Email("user@gmail.com"); // implicit conversion to string
email.ToString();                             // "user@gmail.com"
```

There is deliberately no implicit conversion *from* `string`, because that would bypass validation.

### JSON serialization

`Email` carries a built-in `System.Text.Json` converter. It serializes as a plain string and deserializes back through validation:

```csharp showLineNumbers
var json = JsonSerializer.Serialize(new Email("user@gmail.com"));
// json == "\"user@gmail.com\""

// highlight-next-line
var email = JsonSerializer.Deserialize<Email>("\" User@Gmail.com \"");
// email.Value == "user@gmail.com"

JsonSerializer.Deserialize<Email>("\"not-an-email\""); // throws JsonException
```
