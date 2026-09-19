using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Represents the base class for all entities, providing a unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class CoreEntity<TKey>
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public virtual TKey Id { get; protected set; }
}
