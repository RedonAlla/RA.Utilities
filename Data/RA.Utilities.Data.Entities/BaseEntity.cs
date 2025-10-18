using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for all entities, providing common properties like Id, CreatedAt, and LastModifiedAt.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public virtual Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp of the entity.
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
}
