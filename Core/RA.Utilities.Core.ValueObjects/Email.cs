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
/// Represents a validated and normalized email address.
/// </summary>
/// <remarks>
/// <para>
/// Instances can only be created through <see cref="Email(string?)"/>, <see cref="Parse(string, IFormatProvider?)"/>
/// or <see cref="TryParse(string?, IFormatProvider?, out Email)"/>, so an <see cref="Email"/> that exists is always valid.
/// </para>
/// <para>
/// Addresses are trimmed and lower-cased (invariant culture) on creation, so two emails that differ
/// only by casing or surrounding whitespace are equal.
/// </para>
/// <para>
/// The format check is intentionally permissive (one <c>@</c>, no whitespace, a dotted domain).
/// The only reliable way to verify that an address exists is to send a message to it.
/// </para>
/// </remarks>
[JsonConverter(typeof(ValueObjectJsonConverter<Email>))]
public sealed partial record Email : IValueObject<Email>
{
    /// <summary>
    /// The maximum allowed length of an email address, as defined by RFC 5321 (254 characters).
    /// </summary>
    private const int _maxLength = 254;
    private const string _fieldName = "Email";

    /// <summary>
    /// Gets the normalized (trimmed, lower-cased) email address, for example <c>user@gmail.com</c>.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Gets the part of the address before the <c>@</c> sign, for example <c>user</c>.
    /// </summary>
    public string LocalPart => Value[..Value.IndexOf('@')];

    /// <summary>
    /// Gets the part of the address after the <c>@</c> sign, for example <c>gmail.com</c>.
    /// </summary>
    public string Domain => Value[(Value.IndexOf('@') + 1)..];

    /// <summary>
    /// Validates and normalizes <paramref name="input"/> and creates an <see cref="Email"/> from it.
    /// </summary>
    /// <param name="input">The raw email address, typically user input.</param>
    /// <returns>
    /// A successful result containing the <see cref="Email"/>.
    /// </returns>
    public Email(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.EmailRequired)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.EmailRequired
            });

        string normalized = input.Trim().ToLowerInvariant();

        if (normalized.Length > _maxLength)
            throw new BadRequestException(new ValidationError($"Email address must not exceed {_maxLength} characters.")
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.EmailMaxLength,
                AttemptedValue = normalized,
            });

        if (!FormatRegex().IsMatch(normalized))
            throw new BadRequestException(new ValidationError(BaseErrorMessage.EmailNotValid)
            {
                PropertyName = _fieldName,
                ErrorCode = BaseErrorCode.EmailNotValid,
                AttemptedValue = normalized,
                ExpectedValue = "name@example.com"
            });

        Value = normalized;
    }

    /// <summary>
    /// Converts a string to an <see cref="Email"/> and throws if it is not valid.
    /// Prefer <see cref="Email(string?)"/> in domain code; this exists to support <see cref="IParsable{TSelf}"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <returns>The parsed <see cref="Email"/>.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid email address.</exception>
    public static Email Parse(string s, IFormatProvider? provider) => new(s);

    /// <summary>
    /// Tries to convert a string to an <see cref="Email"/>. Used by ASP.NET Core minimal API route,
    /// query and header binding, where a <see langword="false"/> result produces a 400 response.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Present to satisfy <see cref="IParsable{TSelf}"/>.</param>
    /// <param name="result">The parsed <see cref="Email"/> when successful; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> is a valid email address; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Email result)
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
    /// Implicitly converts an <see cref="Email"/> to its normalized string representation.
    /// This direction is lossless and cannot fail. There is deliberately no conversion from
    /// <see cref="string"/>, because that would bypass <see cref="Email(string?)"/> validation.
    /// </summary>
    /// <param name="email">The email to convert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="email"/> is <see langword="null"/>.</exception>
    public static implicit operator string(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        return email.Value;
    }

    /// <summary>
    /// Returns the normalized email address. Overrides the record's generated
    /// <c>Email { Value = ... }</c> output so the address prints cleanly in logs and string interpolation.
    /// </summary>
    /// <returns>The value of <see cref="Value"/>.</returns>
    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex FormatRegex();
}
