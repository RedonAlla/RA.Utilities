#pragma warning disable CA1308

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects.Converters;

namespace RA.Utilities.Core.ValueObjects;

/// <summary>
/// Represents a validated and normalized currency.
/// </summary>
/// <remarks>
/// <para>
/// Instances can only be created through <see cref="Currency(string?)"/>, <see cref="Parse(string, IFormatProvider?)"/>
/// or <see cref="TryParse(string?, IFormatProvider?, out Currency)"/>, so an <see cref="Currency"/> that exists is always valid.
/// </para>
/// <para>
/// Currencies are trimmed and upper-cased (invariant culture) on creation, so two currencies that differ
/// only by casing or surrounding whitespace are equal.
/// </para>
/// </remarks>
[JsonConverter(typeof(ValueObjectJsonConverter<Currency>))]
public sealed partial record Currency : IValueObject<Currency>
{
    /// <summary>
    /// Length of an currency, as defined by ISO 4217 (3 characters).
    /// </summary>
    private const int _length = 3;

    private const string _fieldName = "Currency";

    /// <summary>
    /// Gets the normalized (trimmed, upper-cased) currency, for example <c>EUR</c>.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Validates and normalizes <paramref name="value"/> and creates an <see cref="Currency"/> from it.
    /// </summary>
    /// <param name="value">The raw currency, typically user input.</param>
    /// <returns>
    /// A successful result containing the <see cref="Currency"/>.
    /// </returns>
    public Currency(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.CurrencyRequired)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.CurrencyRequired
            });

        string normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length != _length)
            throw new BadRequestException(new ValidationError(BaseErrorMessage.CurrencyLength)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.CurrencyLength,
                AttemptedValue = normalized,
                ExpectedValue = "EUR"
            });

        Value = normalized;
    }

    /// <summary>
    /// Converts a string to an <see cref="Currency"/> and throws if it is not valid.
    /// Prefer <see cref="Currency(string?)"/> in domain code; this exists to support <see cref="IParsable{TSelf}"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <returns>The parsed <see cref="Currency"/>.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid currency.</exception>
    public static Currency Parse(string s, IFormatProvider? provider) => new(s);

    /// <summary>
    /// Tries to convert a string to an <see cref="Currency"/>. Used by ASP.NET Core minimal API route,
    /// query and header binding, where a <see langword="false"/> result produces a 400 response.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <param name="result">The parsed <see cref="Currency"/> when successful; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> is a valid currency; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Currency result)
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
    /// Implicitly converts an <see cref="Currency"/> to its normalized string representation.
    /// This direction is lossless and cannot fail. There is deliberately no conversion from
    /// <see cref="string"/>, because that would bypass <see cref="Currency(string?)"/> validation.
    /// </summary>
    /// <param name="currency">The currency to convert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="currency"/> is <see langword="null"/>.</exception>
    public static implicit operator string(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        return currency.Value;
    }
}
