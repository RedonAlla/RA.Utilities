using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RA.Utilities.Integrations.Generators;

/// <summary>
/// An immutable array with value-based equality, used in incremental generator models
/// so that the pipeline can cache and compare inputs by value instead of by reference.
/// </summary>
/// <typeparam name="T">The type of the elements, which must support value equality.</typeparam>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// The empty array.
    /// </summary>
    public static readonly EquatableArray<T> Empty = new(ImmutableArray<T>.Empty);

    /// <summary>
    /// The underlying immutable array.
    /// </summary>
    private readonly ImmutableArray<T> _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableArray{T}"/> struct.
    /// </summary>
    /// <param name="array">The immutable array to wrap.</param>
    public EquatableArray(ImmutableArray<T> array)
    {
        _array = array;
    }

    /// <summary>
    /// Gets a value indicating whether the array is default or empty.
    /// </summary>
    public bool IsDefaultOrEmpty => _array.IsDefaultOrEmpty;

    /// <inheritdoc/>
    public int Count => _array.Length;

    /// <inheritdoc/>
    public T this[int index] => _array[index];

    /// <summary>
    /// Returns an enumerator over the elements.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public ImmutableArray<T>.Enumerator GetEnumerator() => _array.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_array).GetEnumerator();

    /// <inheritdoc/>
    public bool Equals(EquatableArray<T> other) => _array.SequenceEqual(other._array);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = 0;

        foreach (T item in _array)
        {
            hash = (hash * 397) ^ item.GetHashCode();
        }

        return hash;
    }

    /// <summary>
    /// Implicitly converts an immutable array into an equatable array.
    /// </summary>
    /// <param name="array">The immutable array.</param>
    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);

    /// <summary>
    /// Implicitly converts an equatable array back into an immutable array.
    /// </summary>
    /// <param name="array">The equatable array.</param>
    public static implicit operator ImmutableArray<T>(EquatableArray<T> array) => array._array;
}
