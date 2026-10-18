---
title: Value Object Converters
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Conventions
```

The `Conventions` namespace provides EF Core value converters that persist the value objects of [`RA.Utilities.Core.ValueObjects`](../../../core/RA.Utilities.Core.ValueObjects/index.mdx) as columns. Each converter derives from `ValueConverter<TValueObject, string>`.

## 🎯 Purpose

Value objects guarantee validity in memory, but EF Core does not know how to store them. A converter supplies two expressions:

- **Write** — store the normalized `Value` (or, for `Money`, the JSON representation).
- **Read** — construct the value object through its constructor, re-running validation on materialization.

That means invalid data already in the database fails loudly when read instead of entering your domain unchecked.

## Available converters

| Converter | Value object | Stored as | Read-side validation |
|---|---|---|---|
| `EmailConverter` | [`Email`](../../../core/RA.Utilities.Core.ValueObjects/Email.md) | Normalized `Value` string, e.g. `user@example.com` | `new Email(value)` |
| `CurrencyConverter` | [`Currency`](../../../core/RA.Utilities.Core.ValueObjects/Currency.md) | ISO 4217 code, e.g. `EUR` | `new Currency(value)` |
| `SsnConverter` | [`SSN`](../../../core/RA.Utilities.Core.ValueObjects/SSN.md) | Normalized string, e.g. `J01234567R` | `new SSN(value)` |
| `IbanConverter` | [`IBAN`](../../../core/RA.Utilities.Core.ValueObjects/IBAN.md) | Normalized IBAN, e.g. `GB82WEST12345698765432` | `new IBAN(value)` |
| `MoneyConverter` | [`Money`](../../../core/RA.Utilities.Core.ValueObjects/Money.md) | JSON string, e.g. `{"Amount":19.99,"Currency":"EUR"}` | `JsonSerializer.Deserialize<Money>(value)` |

:::note[v10.2.0]

`IbanConverter` was added in v10.2.0 alongside the `IBAN` value object. The other converters are unchanged.

:::

## 🚀 How to Use

### Apply a converter per property

```csharp showLineNumbers
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RA.Utilities.Data.EntityFramework.Conventions;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // highlight-next-line
        builder.Property(c => c.Email).HasConversion(new EmailConverter());
        builder.Property(c => c.Ssn).HasConversion(new SsnConverter());
        builder.Property(c => c.Iban).HasConversion(new IbanConverter());
        builder.Property(c => c.PreferredCurrency).HasConversion(new CurrencyConverter());
    }
}
```

### Apply converters model-wide

If every property of a given value-object type should use the same converter, configure it once in `ConfigureConventions`:

```csharp showLineNumbers
using RA.Utilities.Core.ValueObjects;
using RA.Utilities.Data.EntityFramework.Conventions;

protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Properties<Email>().HaveConversion<EmailConverter>();
    configurationBuilder.Properties<Currency>().HaveConversion<CurrencyConverter>();
    configurationBuilder.Properties<SSN>().HaveConversion<SsnConverter>();
    configurationBuilder.Properties<IBAN>().HaveConversion<IbanConverter>();

    // Money has two supported mappings; choose one:
    configurationBuilder.Properties<Money>().HaveConversion<MoneyConverter>();
}
```

Per-property configuration takes precedence, so you can still map an individual `Money` property with [`HasMoney()`](../Extensions/MoneyConventionExtensions.md) instead.

### Store `Money` as a single JSON column

```csharp showLineNumbers
// highlight-next-line
builder.Property(w => w.Balance).HasConversion(new MoneyConverter());
```

`MoneyConverter` uses `System.Text.Json` with default options. Because `Money` is a record with `Amount` and `Currency`, the stored JSON is:

```json
{ "Amount": 19.99, "Currency": "EUR" }
```

### Store `Money` as two columns

Use [`HasMoney()`](../Extensions/MoneyConventionExtensions.md) instead of `MoneyConverter` when amounts must be queried or aggregated in SQL:

```csharp showLineNumbers
// highlight-next-line
modelBuilder.Entity<Wallet>().HasMoney(w => w.Balance); // Balance_Amount + Balance_Currency
```

## 💡 Choosing a `Money` mapping

| Mapping | Columns | Best for |
|---|---|---|
| `MoneyConverter` | One string column with JSON | Simple persistence; value used as a whole |
| `HasMoney()` | `{Property}_Amount` (decimal) + `{Property}_Currency` (string) | SQL filtering, sorting, indexing, and aggregation on the amount |

Do not configure both for the same property.

## 🧠 Summary

Value converters persist validated value objects without giving up their guarantees: writes store the normalized representation, and reads re-run constructor validation. Apply them per property or model-wide, and pick the `Money` mapping that matches how the data is queried.
