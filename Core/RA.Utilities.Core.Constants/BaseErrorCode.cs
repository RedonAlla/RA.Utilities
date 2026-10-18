namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default error codes used by all microservices.
/// </summary>
public abstract partial class BaseErrorCode
{
    /// <summary>
    /// Error code returned when the 'Email address' field is missing.
    /// </summary>
    public static readonly string EmailRequired = "REQUIRED_EMAIL";

    /// <summary>
    /// Error code returned when the 'Email address' field exceeds the maximum length.
    /// </summary>
    public static readonly string EmailMaxLength = "EMAIL_MAX_LENGTH";

    /// <summary>
    /// Error code returned when the 'Email address' format is invalid.
    /// </summary>
    public static readonly string EmailNotValid = "INVALID_EMAIL";

    /// <summary>
    /// Error code returned when the 'Currency' field is missing.
    /// </summary>
    public static readonly string CurrencyRequired = "REQUIRED_CURRENCY";

    /// <summary>
    /// Error code returned when the 'Currency' field is not exactly 3 characters.
    /// </summary>
    public static readonly string CurrencyLength = "CURRENCY_LENGTH";

    /// <summary>
    /// Error code returned when two 'Currency' values should be the same.
    /// </summary>
    public static readonly string CurrencyMismatch = "CURRENCY_MISMATCH";

    /// <summary>
    /// Error code returned when the 'Amount' is negative (less than 0).
    /// </summary>
    public static readonly string PositiveMoney = "POSITIVE_MONEY";

    /// <summary>
    /// Error code returned when the 'SSN' field is missing.
    /// </summary>
    public static readonly string SsnRequired = "REQUIRED_SSN";

    /// <summary>
    /// Error code returned when the 'SSN' field is not exactly 10 characters.
    /// </summary>
    public static readonly string SsnLength = "SSN_LENGTH";

    /// <summary>
    /// Error code returned when the 'SSN' format is invalid.
    /// </summary>
    public static readonly string SsnNotValid = "NOT_VALID_SSN";

    /// <summary>
    /// Error code returned when the 'IBAN' field is missing.
    /// </summary>
    public static readonly string IbanRequired = "REQUIRED_IBAN";

    /// <summary>
    /// Error code returned when the 'IBAN' total length does not match the expected length for its country.
    /// </summary>
    public static readonly string IbanLength = "IBAN_LENGTH";

    /// <summary>
    /// Error code returned when the 'IBAN' format or structure is invalid.
    /// </summary>
    public static readonly string IbanNotValid = "NOT_VALID_IBAN";

    /// <summary>
    /// Error code returned when the 'IBAN' check digits fail the ISO 7064 mod-97 validation.
    /// </summary>
    public static readonly string IbanCheckDigits = "IBAN_CHECK_DIGITS";

}
