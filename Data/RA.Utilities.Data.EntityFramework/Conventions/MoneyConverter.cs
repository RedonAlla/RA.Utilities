using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Data.EntityFramework.Conventions;

/// <summary>
/// Converts between the <see cref="Money"/> value object and its stored string representation.
/// </summary>
public class MoneyConverter : ValueConverter<Money, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoneyConverter"/> class.
    /// </summary>
    public MoneyConverter()
        : base(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<Money>(v, (JsonSerializerOptions)null)
        )
    {
    }
}
