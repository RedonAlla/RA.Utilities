#pragma warning disable CA1052

namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default error codes used by all microservices.
/// </summary>
public class BaseErrorCode
{
    /// <summary>
    /// Error code returned when the 'Email address' field is missing.
    /// </summary>
    public const string EmailRequired = "REQUIRED_EMAIL";

    /// <summary>
    /// Error code returned when the 'Email address' field exceeds the maximum length.
    /// </summary>
    public const string EmailMaxLength = "EMAIL_MAX_LENGTH";

    /// <summary>
    /// Error code returned when the 'Email address' format is invalid.
    /// </summary>
    public const string EmailNotValid = "INVALID_EMAIL";

    /// <summary>
    /// Error code returned when the 'Currency' field is missing.
    /// </summary>
    public const string CurrencyRequired = "REQUIRED_CURRENCY";

    /// <summary>
    /// Error code returned when the 'Currency' field is not exactly 3 characters.
    /// </summary>
    public const string CurrencyLength = "CURRENCY_LENGTH";

    /// <summary>
    /// Error code returned when two 'Currency' should me the same.
    /// </summary>
    public const string CurrencyMismatch = "CURRENCY_MISMATCH";

    /// <summary>
    /// Error code returned when the 'Amount' it is negative (less then 0).
    /// </summary>
    public const string PositiveMoney = "POSITIVE_MONEY";

    /// <summary>
    /// Error code returned when the 'SSN' field is missing.
    /// </summary>
    public const string SsnRequired = "REQUIRED_SSN";

    /// <summary>
    /// Error code returned when the 'Currency' field is not exactly 10 characters.
    /// </summary>
    public const string SsnLength = "SSN_LENGTH";

    /// <summary>
    /// Error code returned when the 'SSN' format is invalid.
    /// </summary>
    public const string SsnNotValid = "NOT_VALID_SSN";

}
