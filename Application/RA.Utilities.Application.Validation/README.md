# RA.Utilities.Application.Validation

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Application.Validation.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Application.Validation/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Application.Validation.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Application.Validation/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Application/FeatureValidation/)

`RA.Utilities.Application.Validation` provides reusable validators and helpers built on [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) for validating incoming requests.

Run every registered validator for a request in parallel, aggregate the failures, and convert them into a standardized `BadRequestException` — the payload that `RA.Utilities.Api` middleware translates into a consistent **HTTP 400** response — without writing the orchestration code yourself.

## ✨ Key Features
* **Parallel validation orchestration**: `ValidationUtilities.ValidateAsync` executes all validators for a request concurrently — each with its own `ValidationContext`, so results cannot leak between validators — and flattens the failures into a single array, forwarding your `CancellationToken` to every validator.
* **Standardized error payloads**: `ValidationUtilities.CreateValidationErrorResult` maps FluentValidation's `ValidationFailure[]` into a `BadRequestException` carrying structured `ValidationError` entries (`PropertyName`, `AttemptedValue`, `ErrorCode`).
* **Value-object-backed rules**: `MustBeValidCurrency()`, `MustBeValidEmail()` and `MustBeValidSsn()` validate `string?` properties through the value objects of `RA.Utilities.Core.ValueObjects`, so request validation and the domain share one set of rules.
* **ISO 4217 format rule**: `MustMatchesCurrencyFormat()` checks the currency shape (3 uppercase letters) with a source-generated compiled regex.

## Installation
You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Application.Validation
```

```powershell
Install-Package RA.Utilities.Application.Validation
```

## Usage

### Run all validators for a request

```csharp
using FluentValidation;
using FluentValidation.Results;
using RA.Utilities.Application.Validation.Extensions;
using RA.Utilities.Application.Validation.Utilities;

public record CreateOrderRequest(string Currency, decimal Amount);

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Currency).MustMatchesCurrencyFormat();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(
    request, [new CreateOrderValidator()], CancellationToken.None);

if (failures.Length > 0)
{
    throw ValidationUtilities.CreateValidationErrorResult(failures);
}
```

The utilities are static — no configuration of their own is required; validators are supplied by your own DI container.

### Value-object-backed validation rules

The `MustBeValid*` rules in `RA.Utilities.Application.Validation.Extensions` construct the matching value object (`Currency`, `Email`, `SSN`) behind the scenes. Any input the value object rejects fails the rule for that property path:

```csharp
using RA.Utilities.Application.Validation.Extensions;

public record RegisterUserRequest(string Email, string Currency, string Ssn);

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

Because the value object constructors reject `null`, empty and whitespace input, these rules also fail on missing values — unlike `MustMatchesCurrencyFormat()`, where `null` passes and requiredness is left to `NotNull()` / `NotEmpty()`.

The failure message is the generic `BadRequestException` message ("The request is invalid."), not the value object's specific message; chain `WithMessage(...)` if you want a property-specific message.

## Dependencies
* [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) (12.1.1)
* `RA.Utilities.Core.Exceptions`
* `RA.Utilities.Core.ValueObjects`

## Additional documentation
For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/Application/FeatureValidation/).

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.
