using System;

namespace RA.Utilities.Core.ValueObjects;

/// <summary>
/// Defines a validated value object with a single normalized string representation.
/// </summary>
/// <typeparam name="TSelf">The implementing value object type.</typeparam>
public interface IValueObject<TSelf> : IParsable<TSelf>
    where TSelf : IValueObject<TSelf>
{
    /// <summary>
    /// Gets the normalized string representation of the value object, for example <c>EUR</c> or <c>user@gmail.com</c>.
    /// </summary>
    string Value { get; }
}
