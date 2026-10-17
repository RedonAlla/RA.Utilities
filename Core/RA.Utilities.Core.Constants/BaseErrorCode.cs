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
    /// Error code returned when two 'Currency' should me the same.
    /// </summary>
    public static readonly string CurrencyMismatch = "CURRENCY_MISMATCH";

    /// <summary>
    /// Error code returned when the 'Amount' it is negative (less then 0).
    /// </summary>
    public static readonly string PositiveMoney = "POSITIVE_MONEY";

    /// <summary>
    /// Error code returned when the 'SSN' field is missing.
    /// </summary>
    public static readonly string SsnRequired = "REQUIRED_SSN";

    /// <summary>
    /// Error code returned when the 'Currency' field is not exactly 10 characters.
    /// </summary>
    public static readonly string SsnLength = "SSN_LENGTH";

    /// <summary>
    /// Error code returned when the 'SSN' format is invalid.
    /// </summary>
    public static readonly string SsnNotValid = "NOT_VALID_SSN";

}
