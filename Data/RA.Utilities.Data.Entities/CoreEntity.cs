using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Represents the base class for all entities, providing a unique identifier.
/// </summary>
public abstract class CoreEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public virtual Guid Id { get; set; }
}
