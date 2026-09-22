#pragma warning disable CA1052

namespace RA.Utilities.Core.Constants;

/// <summary>
/// General default error messages used by all microservices.
/// </summary>
public class BaseErrorMessage
{
    /// <summary>
    /// Error message returned when the 'Email address' field is missing.
    /// </summary>
    public const string EmailRequired = "The 'Email address' field is required.";

    /// <summary>
    /// Error message returned when the 'Email address' format is invalid.
    /// </summary>
    public const string EmailNotValid = "'Email address' format is invalid.";

    /// <summary>
    /// Error message returned when the 'Currency' field is missing.
    /// </summary>
    public const string CurrencyRequired = "The 'Currency' field is required.";

    /// <summary>
    /// Error message returned when the 'Currency' field is not exactly 3 characters.
    /// </summary>
    public const string CurrencyLength = "'Currency' must be exact 3 characters.";

    /// <summary>
    /// Error message returned when two 'Currency' should me the same.
    /// </summary>
    public const string CurrencyMismatch = "Currency mismatch";

    /// <summary>
    /// Error message returned when the 'Amount' it is negative (less then 0).
    /// </summary>
    public const string PositiveMoney = "POSITIVE_MONEY";

    /// <summary>
    /// Error message returned when the 'SSN' field is missing.
    /// </summary>
    public const string SsnRequired = "The 'SSN' field is required.";

    /// <summary>
    /// Error message returned when the 'Currency' field is not exactly 10 characters.
    /// </summary>
    public const string SsnLength = "'SSN' must be exact 10 characters.";

    /// <summary>
    /// Error message returned when the 'SSN' format is invalid.
    /// </summary>
    public const string SsnNotValid = "'SSN' format is invalid.";
}
