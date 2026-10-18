using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Data.EntityFramework.Conventions;

/// <summary>
/// Converts between the <see cref="IBAN"/> value object and its stored string representation.
/// </summary>
public class IbanConverter : ValueConverter<IBAN, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IbanConverter"/> class.
    /// </summary>
    public IbanConverter()
        : base(
            v => v.Value,
            v => new IBAN(v)
        )
    {
    }
}
