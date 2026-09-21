#pragma warning disable CA1308

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects.Converters;

namespace RA.Utilities.Core.ValueObjects;

/// <summary>
/// Represents a validated and normalized SSN.
/// </summary>
/// <remarks>
/// <para>
/// Instances can only be created through <see cref="SSN(string?)"/>, <see cref="Parse(string, IFormatProvider?)"/>
/// or <see cref="TryParse(string?, IFormatProvider?, out SSN)"/>, so an <see cref="SSN"/> that exists is always valid.
/// </para>
/// <para>
/// SSN are trimmed and upper-cased (invariant culture) on creation, so two SSNs that differ
/// only by casing or surrounding whitespace are equal.
/// </para>
/// </remarks>
[JsonConverter(typeof(ValueObjectJsonConverter<SSN>))]
public sealed partial record SSN : IValueObject<SSN>
{
    private const int _maxLength = 10;
    private const string _fieldName = "SSN";

    /// <summary>
    /// Gets the normalized (trimmed, upper-cased) SSN, for example <c>J01234567R</c>.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Validates and normalizes <paramref name="input"/> and creates an <see cref="SSN"/> from it.
    /// </summary>
    /// <param name="input">The raw SSN, typically user input.</param>
    /// <returns>
    /// A successful result containing the <see cref="SSN"/>.
    /// </returns>
    public SSN(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.SsnRequired)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.SsnRequired
            });

        string normalized = input.Trim().ToUpperInvariant();

        if (normalized.Length > _maxLength)
            throw new BadRequestException(new ValidationError(BaseErrorMessage.SsnLength)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.SsnLength,
                AttemptedValue = normalized,
            });

        if (!FormatRegex().IsMatch(normalized))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.SsnNotValid)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.SsnNotValid,
                AttemptedValue = normalized,
                ExpectedValue = "J01234567R"
            });

        Value = normalized;
    }

    /// <summary>
    /// Converts a string to an <see cref="SSN"/> and throws if it is not valid.
    /// Prefer <see cref="SSN(string?)"/> in domain code; this exists to support <see cref="IParsable{TSelf}"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <returns>The parsed <see cref="SSN"/>.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid SSN.</exception>
    public static SSN Parse(string s, IFormatProvider? provider) => new(s);

    /// <summary>
    /// Tries to convert a string to an <see cref="SSN"/>. Used by ASP.NET Core minimal API route,
    /// query and header binding, where a <see langword="false"/> result produces a 400 response.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <param name="result">The parsed <see cref="SSN"/> when successful; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> is a valid SSN; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out SSN result)
    {
        try
        {
            result = new(s!);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Implicitly converts an <see cref="SSN"/> to its normalized string representation.
    /// This direction is lossless and cannot fail. There is deliberately no conversion from
    /// <see cref="string"/>, because that would bypass <see cref="SSN(string?)"/> validation.
    /// </summary>
    /// <param name="SSN">The SSN to convert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="SSN"/> is <see langword="null"/>.</exception>
    public static implicit operator string(SSN SSN)
    {
        ArgumentNullException.ThrowIfNull(SSN);
        return SSN.Value;
    }

    /// <summary>
    /// Returns the normalized SSN address. Overrides the record's generated
    /// <c>SSN { Value = ... }</c> output so the address prints cleanly in logs and string interpolation.
    /// </summary>
    /// <returns>The value of <see cref="Value"/>.</returns>
    public override string ToString() => Value;

    [GeneratedRegex(@"^[A-Z][0-9]{8}[A-Z]$", RegexOptions.CultureInvariant)]
    private static partial Regex FormatRegex();
}
