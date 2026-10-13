using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Data.EntityFramework.Conventions;

/// <summary>
/// Converts between the <see cref="Currency"/> value object and its stored string representation.
/// </summary>
public class CurrencyConverter : ValueConverter<Currency, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyConverter"/> class.
    /// </summary>
    public CurrencyConverter()
        : base(
            v => v.Value,
            v => new Currency(v)
        )
    {
    }
}
