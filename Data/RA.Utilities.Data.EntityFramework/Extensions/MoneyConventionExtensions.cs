using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RA.Utilities.Core.ValueObjects;
using RA.Utilities.Data.EntityFramework.Conventions;

namespace RA.Utilities.Data.EntityFramework.Extensions;

/// <summary>
/// Configures <see cref="Money"/> properties to be stored as two separate database columns:
/// a decimal <c>Amount</c> column and a string <c>Currency</c> column.
/// </summary>
/// <remarks>
/// <para>
/// EF Core value converters map a single property to a single column, so <see cref="Money"/> cannot be split
/// across two columns with a converter such as <see cref="MoneyConverter"/>. This convention maps the property
/// as an EF Core complex property instead, giving <see cref="Money.Amount"/> and <see cref="Money.Currency"/>
/// a column each, with <see cref="CurrencyConverter"/> handling the <see cref="Currency"/> value object.
/// </para>
/// </remarks>
public static class MoneyConventionExtensions
{
    /// <summary>
    /// Maps the <paramref name="money"/> property to two database columns:
    /// <see cref="Money.Amount"/> (decimal) and <see cref="Money.Currency"/> (string).
    /// </summary>
    /// <typeparam name="T">The entity type that declares the <paramref name="money"/> property.</typeparam>
    /// <param name="entityTypeBuilder">The builder of the entity type that declares the property.</param>
    /// <param name="money">The <see cref="Money"/> property to map.</param>
    /// <returns>
    /// The complex property builder, so column names and other facets can still be configured.
    /// </returns>
    public static ComplexPropertyBuilder<Money> HasMoney<T>(
        this EntityTypeBuilder<T> entityTypeBuilder,
        Expression<Func<T, Money?>> money)
        where T : class
    {
        ComplexPropertyBuilder<Money> moneyBuilder = entityTypeBuilder.ComplexProperty(money);

        // Complex type property discovery skips get-only members, so both must be mapped explicitly
        // for the Money(decimal, Currency) constructor to bind during materialization.
        moneyBuilder.Property(m => m.Amount);
        moneyBuilder.Property(m => m.Currency)
            .HasConversion(new CurrencyConverter());

        return moneyBuilder;
    }
}
