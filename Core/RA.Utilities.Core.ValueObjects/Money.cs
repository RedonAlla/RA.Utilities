using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Core.ValueObjects;

/// <summary>
/// Represents a monetary amount in a specific currency.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Gets the numeric value of the amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency associated with the amount.
    /// </summary>
    public Currency Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> record.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency of the amount.</param>
    public Money(decimal amount, Currency currency) =>
        (Amount, Currency) = (amount, currency);

    /// <summary>
    /// Creates a value representing a positive amount.
    /// </summary>
    /// <param name="amount">The amount to validate.</param>
    /// <param name="currency">The currency of the amount.</param>
    /// <returns>A validated positive <see cref="Money"/> instance.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="amount"/> is negative.</exception>
    public static Money PositiveMoney(decimal amount, Currency currency) =>
        amount < 0
            ? throw new BadRequestException(new ValidationError(BaseErrorMessage.PositiveMoney)
            {
                PropertyName = "Amount",
                ErrorCode = BaseErrorCode.PositiveMoney,
                AttemptedValue = amount,
            })
            : new(amount, currency);

    /// <summary>
    /// Adds another monetary amount to the current instance.
    /// </summary>
    /// <param name="other">The amount to add. It must use the same currency as the current instance.</param>
    /// <returns>A new <see cref="Money"/> instance containing the sum.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="other"/> uses a different currency.</exception>
    public Money Add(Money other) =>
        Currency == other.Currency
            ? new(Amount + other.Amount, Currency)
            : throw new BadRequestException(new ValidationError(BaseErrorMessage.CurrencyMismatch)
            {
                PropertyName = "Currency",
                ErrorCode = BaseErrorCode.CurrencyMismatch,
                AttemptedValue = other.Currency,
                ExpectedValue = Currency
            });

    /// <summary>
    /// Returns a string that represents the current money value.
    /// </summary>
    /// <returns>A formatted string containing the amount and currency.</returns>
    public override string ToString() => $"{Amount} {Currency}";
}
