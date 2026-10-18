namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default error messages used by all microservices.
/// </summary>
public abstract partial class BaseErrorMessage
{
    /// <summary>
    /// Error message returned when the 'Email address' field is missing.
    /// </summary>
    public static readonly string EmailRequired = "The 'Email address' field is required.";

    /// <summary>
    /// Error message returned when the 'Email address' format is invalid.
    /// </summary>
    public static readonly string EmailNotValid = "'Email address' format is invalid.";

    /// <summary>
    /// Error message returned when the 'Currency' field is missing.
    /// </summary>
    public static readonly string CurrencyRequired = "The 'Currency' field is required.";

    /// <summary>
    /// Error message returned when the 'Currency' field is not exactly 3 characters.
    /// </summary>
    public static readonly string CurrencyLength = "'Currency' must be exact 3 characters.";

    /// <summary>
    /// Error message returned when two 'Currency' values should be the same.
    /// </summary>
    public static readonly string CurrencyMismatch = "Currency mismatch";

    /// <summary>
    /// Error message returned when the 'Amount' is negative (less than 0).
    /// </summary>
    public static readonly string PositiveMoney = "POSITIVE_MONEY";

    /// <summary>
    /// Error message returned when the 'SSN' field is missing.
    /// </summary>
    public static readonly string SsnRequired = "The 'SSN' field is required.";

    /// <summary>
    /// Error message returned when the 'SSN' field is not exactly 10 characters.
    /// </summary>
    public static readonly string SsnLength = "'SSN' must be exact 10 characters.";

    /// <summary>
    /// Error message returned when the 'SSN' format is invalid.
    /// </summary>
    public static readonly string SsnNotValid = "'SSN' format is invalid.";

    /// <summary>
    /// Error message returned when the 'IBAN' field is missing.
    /// </summary>
    public static readonly string IbanRequired = "The 'IBAN' field is required.";

    /// <summary>
    /// Error message returned when the 'IBAN' total length does not match the expected length for its country.
    /// </summary>
    public static readonly string IbanLength = "'IBAN' length does not match the expected length for its country.";

    /// <summary>
    /// Error message returned when the 'IBAN' format or structure is invalid.
    /// </summary>
    public static readonly string IbanNotValid = "'IBAN' format is invalid.";

    /// <summary>
    /// Error message returned when the 'IBAN' check digits fail the ISO 7064 mod-97 validation.
    /// </summary>
    public static readonly string IbanCheckDigits = "'IBAN' check digits are invalid.";
}
