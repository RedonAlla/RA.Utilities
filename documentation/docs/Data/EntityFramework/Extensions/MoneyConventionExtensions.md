---
title: MoneyConventionExtensions
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Data.EntityFramework.Extensions
```

`MoneyConventionExtensions` maps a [`Money`](../../../core/RA.Utilities.Core.ValueObjects/Money.md) property to **two separate database columns** — a decimal `Amount` column and a string `Currency` column (through [`CurrencyConverter`](../../EntityFramework/Conventions/ValueObjectConverters)) — instead of storing the whole value in a single column.

## 🎯 Purpose and Design

EF Core value converters map a single property to a single column, so a `Money` cannot be split across two columns with a converter such as [`MoneyConverter`](../../EntityFramework/Conventions/ValueObjectConverters). `HasMoney()` maps the property as an EF Core **complex property** instead, giving `Money.Amount` and `Money.Currency` a column each.

Because complex type property discovery skips get-only members, `HasMoney()` maps both properties explicitly so the `Money(decimal, Currency)` constructor binds during materialization.

## 🚀 How to Use

```csharp showLineNumbers
using RA.Utilities.Data.EntityFramework.Extensions;

public class Wallet
{
    public int Id { get; set; }
    public Money? Balance { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // highlight-next-line
    modelBuilder.Entity<Wallet>().HasMoney(w => w.Balance);
}
```

### Signature

```csharp
extension<T>(EntityTypeBuilder<T> entityTypeBuilder) where T : class
{
    public ComplexPropertyBuilder<Money> HasMoney(Expression<Func<T, Money?>> money)
}
```

The method is declared as a C# 14 extension member on `EntityTypeBuilder<T>`; the calling syntax is unchanged.

The returned `ComplexPropertyBuilder<Money>` lets you keep configuring column names and other facets:

```csharp showLineNumbers
modelBuilder.Entity<Wallet>()
    .HasMoney(w => w.Balance)
    .Property(m => m.Amount)
    .HasColumnType("decimal(18,4)");
```

## 📋 Behavior

* **Column naming** — columns are named after the property, e.g. `Balance_Amount` and `Balance_Currency`.
* **Amount column type** — the provider default for `decimal` (`decimal(18,2)` on SQL Server, `TEXT` on SQLite) unless you configure it explicitly.
* **Required and optional** — supports both `Money` and `Money?` properties; optional values are stored as `NULL` in both columns.
* **Multiple amounts per entity** — several `Money` properties on the same entity each get their own column pair.
* **Updates** — because `Money` is immutable, updates persist by replacing the instance on a tracked entity.
* **Single-column alternative** — the existing [`MoneyConverter`](../../EntityFramework/Conventions/ValueObjectConverters) (single JSON string column) is unchanged and remains available for single-column mappings.

## 🧠 Summary

`HasMoney()` is the two-column counterpart to [`MoneyConverter`](../../EntityFramework/Conventions/ValueObjectConverters): prefer it when you want to query, index, or aggregate amounts in SQL, and `MoneyConverter` when a single JSON string column is enough.
