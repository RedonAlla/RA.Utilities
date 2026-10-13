using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Data.EntityFramework.Conventions;

/// <summary>
/// Converts between the <see cref="SSN"/> value object and its stored string representation.
/// </summary>
public class SsnConverter : ValueConverter<SSN, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SsnConverter"/> class.
    /// </summary>
    public SsnConverter()
        : base(
            v => v.Value,
            v => new SSN(v)
        )
    {
    }
}
