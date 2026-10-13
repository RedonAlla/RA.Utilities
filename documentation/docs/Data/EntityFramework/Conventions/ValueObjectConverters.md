---
title: Value Object Converters
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Conventions
```

The `Conventions` namespace provides EF Core value converters that map the value objects of [`RA.Utilities.Core.ValueObjects`](../../../core/RA.Utilities.Core.ValueObjects/index.mdx) to database columns. Each converter derives from `ValueConverter<TValueObject, string>`.

## Available Converters

| Converter            | Value object                                            | Stored as |
|----------------------|---------------------------------------------------------|-----------|
| `CurrencyConverter`  | [`Currency`](../../../core/RA.Utilities.Core.ValueObjects/Currency.md) | Normalized `Value` string, e.g. `EUR` |
| `EmailConverter`     | [`Email`](../../../core/RA.Utilities.Core.ValueObjects/Email.md)       | Normalized `Value` string, e.g. `user@gmail.com` |
| `SsnConverter`       | [`SSN`](../../../core/RA.Utilities.Core.ValueObjects/SSN.md)           | Normalized `Value` string, e.g. `J01234567R` |
| `MoneyConverter`     | [`Money`](../../../core/RA.Utilities.Core.ValueObjects/Money.md)       | JSON string of the whole value, e.g. `{"Amount":19.99,"Currency":"EUR"}` |

The string-based converters round-trip through the value object constructors, so invalid data coming back from the database fails loudly on materialization instead of leaking into the domain.

## 🚀 How to Use

Apply a converter with `HasConversion` when configuring a property:

```csharp showLineNumbers
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RA.Utilities.Data.EntityFramework.Conventions;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // highlight-next-line
        builder.Property(u => u.Email).HasConversion(new EmailConverter());
        builder.Property(u => u.Ssn).HasConversion(new SsnConverter());
        builder.Property(u => u.PreferredCurrency).HasConversion(new CurrencyConverter());
    }
}
```

```csharp showLineNumbers
// highlight-next-line
builder.Property(w => w.Balance).HasConversion(new MoneyConverter()); // single string column
```

## 💡 Choosing a `Money` Mapping

`MoneyConverter` stores the whole `Money` in a single JSON string column. If you prefer two columns — a decimal `Amount` and a string `Currency` — use the [`HasMoney()`](../../EntityFramework/Extensions/MoneyConventionExtensions) convention instead, which maps the property as an EF Core complex property:

```csharp showLineNumbers
// highlight-next-line
modelBuilder.Entity<Wallet>().HasMoney(w => w.Balance); // Balance_Amount + Balance_Currency columns
```

## 🧠 Summary

Value converters persist validated value objects without giving up their guarantees: writes store the normalized representation, and reads re-run the constructor validation. Use the string converters for the string-based value objects, `MoneyConverter` for single-column `Money` storage, and `HasMoney()` when you want `Money` split across two columns.
