using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities, providing common properties like creation and modification timestamps,
/// and inheriting the unique identifier from <see cref="CoreEntity"/>.
/// </summary>
public abstract class BaseEntity : CoreEntity
{
    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp of the entity.
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
}
