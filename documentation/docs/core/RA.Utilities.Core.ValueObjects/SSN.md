---
title: SSN
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Core.ValueObjects
```

The `SSN` value object represents a validated social security number. Values are trimmed and upper-cased on creation, must not exceed 10 characters, and must match the format `^[A-Z][0-9]{8}[A-Z]$` — a letter, eight digits, and a letter (e.g., `J01234567R`).

## 🎯 Purpose

Personal identifiers must be exact: a single wrong character changes who the record belongs to. `SSN` validates the structure of the number once, at the boundary, and normalizes casing so lookups and equality checks behave predictably.

## Properties

| Property  | Type     | Description                                  |
|-----------|----------|----------------------------------------------|
| **Value** | `string` | The normalized SSN, e.g. `J01234567R`.       |

## Validation rules

| Rule                     | ErrorCode (from `BaseErrorCode`)     | Notes                                       |
|--------------------------|--------------------------------------|---------------------------------------------|
| Missing or whitespace    | `SsnRequired` (`REQUIRED_SSN`)       | `PropertyName` is `SSN`                     |
| Longer than 10 chars     | `SsnLength` (`SSN_LENGTH`)           | `AttemptedValue` contains the input         |
| Invalid format           | `SsnNotValid` (`NOT_VALID_SSN`)      | `ExpectedValue` is `J01234567R`             |

The format requires exactly 10 characters matching `^[A-Z][0-9]{8}[A-Z]$`: a letter, eight digits, a letter. Values of 10 characters that do not match the pattern fail with `SsnNotValid`; values longer than 10 characters fail with `SsnLength`.

All failures are reported as a `BadRequestException` whose `Errors` collection contains a single `ValidationError`.

## 🚀 How to Use

### Create from user input

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;

// highlight-next-line
var ssn = new SSN("  j01234567r  ");

ssn.Value; // "J01234567R"
```

### Parse and TryParse

`SSN` implements `IParsable<SSN>`, which enables binding in ASP.NET Core minimal APIs:

```csharp showLineNumbers
// highlight-next-line
app.MapGet("/employees/{ssn}", (SSN ssn) => GetEmployee(ssn));

var parsed = SSN.Parse("j01234567r", provider: null); // SSN with Value "J01234567R"
```

### Equality and conversions

```csharp showLineNumbers
new SSN(" j01234567r ") == new SSN("J01234567R"); // true — value equality

string value = new SSN("J01234567R"); // implicit conversion to string
ssn.ToString();                          // "J01234567R"
```

There is deliberately no implicit conversion *from* `string`, because that would bypass validation.

### JSON serialization

`SSN` carries a built-in `System.Text.Json` converter. It serializes as a plain string and deserializes back through validation:

```csharp showLineNumbers
var json = JsonSerializer.Serialize(new SSN("J01234567R"));
// json == "\"J01234567R\""

// highlight-next-line
var ssn = JsonSerializer.Deserialize<SSN>("\"j01234567r\"");
// ssn.Value == "J01234567R"

JsonSerializer.Deserialize<SSN>("\"0123456789\""); // throws JsonException
```
