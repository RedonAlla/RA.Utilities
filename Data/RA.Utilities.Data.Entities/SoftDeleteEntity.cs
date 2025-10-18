using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that support soft deletion, inheriting common properties from <see cref="BaseEntity"/>.
/// </summary>
public abstract class SoftDeleteEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
