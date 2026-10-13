using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Data.EntityFramework.Conventions;

/// <summary>
/// Converts between the <see cref="Email"/> value object and its stored string representation.
/// </summary>
public class EmailConverter : ValueConverter<Email, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailConverter"/> class.
    /// </summary>
    public EmailConverter()
        : base(
            v => v.Value,
            v => new Email(v)
        )
    {
    }
}
