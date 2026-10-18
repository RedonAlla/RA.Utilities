#pragma warning disable S101

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects.Converters;

namespace RA.Utilities.Core.ValueObjects;

/// <summary>
/// Represents a validated and normalized IBAN (International Bank Account Number).
/// </summary>
/// <remarks>
/// <para>
/// Instances can only be created through <see cref="IBAN(string?)"/>, <see cref="Parse(string, IFormatProvider?)"/>
/// or <see cref="TryParse(string?, IFormatProvider?, out IBAN)"/>, so an <see cref="IBAN"/> that exists is always valid.
/// </para>
/// <para>
/// Structural validation is a single regular expression: a two-letter country code, two check digits
/// and a 9-30 character alphanumeric BBAN (13-34 characters in total), where groups may optionally be
/// separated by a space, apostrophe, backslash or hyphen as in the paper format
/// (<c>GB82 WEST 1234 5698 7654 32</c>). The check digits must then satisfy the ISO 7064 mod-97 test,
/// which also guarantees they are in the range 02-98.
/// </para>
/// <para>
/// IBANs are trimmed, stripped of whitespace and separator characters and upper-cased (invariant culture)
/// on creation, so two IBANs that differ only by casing or grouping separators are equal.
/// </para>
/// </remarks>
[JsonConverter(typeof(ValueObjectJsonConverter<IBAN>))]
public sealed partial record IBAN : IValueObject<IBAN>
{
    private const string _fieldName = "IBAN";

    /// <summary>
    /// Gets the normalized (trimmed, separator-free, upper-cased) IBAN, for example <c>GB82WEST12345698765432</c>.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Gets the ISO 3166-1 alpha-2 country code of the IBAN, for example <c>GB</c> for
    /// <c>GB82WEST12345698765432</c>.
    /// </summary>
    public string CountryCode => Value[..2];

    /// <summary>
    /// Validates and normalizes <paramref name="value"/> and creates an <see cref="IBAN"/> from it.
    /// </summary>
    /// <param name="value">The raw IBAN, typically user input.</param>
    /// <returns>
    /// A successful result containing the <see cref="IBAN"/>.
    /// </returns>
    public IBAN(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.IbanRequired)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.IbanRequired
            });

        string normalized = Normalize(value);

        if (!IbanRegex().IsMatch(normalized))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.IbanNotValid)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.IbanNotValid,
                AttemptedValue = normalized,
                ExpectedValue = "2a2n[9-30c] in 2 to 7 groups of 3 to 5 characters"
            });

        string expectedCheckDigits = ComputeCheckDigits(normalized);
        if (normalized[2] != expectedCheckDigits[0] || normalized[3] != expectedCheckDigits[1])
            throw new BadRequestException(new ValidationError(BaseErrorMessage.IbanCheckDigits)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.IbanCheckDigits,
                AttemptedValue = normalized,
                ExpectedValue = expectedCheckDigits
            });

        Value = normalized;
    }

    /// <summary>
    /// Converts a string to an <see cref="IBAN"/> and throws if it is not valid.
    /// Prefer <see cref="IBAN(string?)"/> in domain code; this exists to support <see cref="IParsable{TSelf}"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <returns>The parsed <see cref="IBAN"/>.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid IBAN.</exception>
    public static IBAN Parse(string s, IFormatProvider? provider) => new(s);

    /// <summary>
    /// Tries to convert a string to an <see cref="IBAN"/>. Used by ASP.NET Core minimal API route,
    /// query and header binding, where a <see langword="false"/> result produces a 400 response.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <param name="result">The parsed <see cref="IBAN"/> when successful; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> is a valid IBAN; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out IBAN result)
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
    /// Implicitly converts an <see cref="IBAN"/> to its normalized string representation.
    /// This direction is lossless and cannot fail. There is deliberately no conversion from
    /// <see cref="string"/>, because that would bypass <see cref="IBAN(string?)"/> validation.
    /// </summary>
    /// <param name="iban">The IBAN to convert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="iban"/> is <see langword="null"/>.</exception>
    public static implicit operator string(IBAN iban)
    {
        ArgumentNullException.ThrowIfNull(iban);
        return iban.Value;
    }

    /// <summary>
    /// Returns the normalized IBAN. Overrides the record's generated
    /// <c>IBAN { Value = ... }</c> output so the value prints cleanly in logs and string interpolation.
    /// </summary>
    /// <returns>The value of <see cref="Value"/>.</returns>
    public override string ToString() => Value;

    /// <summary>
    /// International Bank Account Number structure: a two-letter country code, two check digits and a
    /// 9-30 character alphanumeric BBAN, grouped in 3 to 5 character blocks plus an optional 1 to 3 character
    /// tail. Groups may be separated by a space, apostrophe, backslash or hyphen (paper format).
    /// </summary>
    [GeneratedRegex(
        @"^([A-Z]{2}[ '\\-]?[0-9]{2})(?=(?:[ '\\-]?[A-Z0-9]){9,30}$)((?:[ '\\-]?[A-Z0-9]{3,5}){2,7})([ '\\-]?[A-Z0-9]{1,3})?$",
        RegexOptions.CultureInvariant)]
    private static partial Regex IbanRegex();

    /// <summary>
    /// Uppercases the input and removes every grouping separator (whitespace, apostrophe, backslash and hyphen),
    /// producing the canonical compact IBAN.
    /// </summary>
    private static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (char character in value)
        {
            if (char.IsWhiteSpace(character) || character is '\'' or '\\' or '-')
                continue;

            builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }

    private static string ComputeCheckDigits(string iban)
    {
        int remainder = 0;
        for (int i = 4; i < iban.Length; i++)
            remainder = Append(remainder, iban[i]);
        remainder = Append(remainder, iban[0]);
        remainder = Append(remainder, iban[1]);
        remainder = remainder * 10 % 97;
        remainder = remainder * 10 % 97;

        return (98 - remainder).ToString("00", CultureInfo.InvariantCulture);

        static int Append(int remainder, char character)
        {
            int value = char.IsAsciiDigit(character) ? character - '0' : character - 'A' + 10;
            int factor = value < 10 ? 10 : 100;
            return (remainder * factor + value) % 97;
        }
    }
}
