using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for writable entities that track creation and modification timestamps,
/// inheriting the unique identifier from <see cref="CoreEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class WriteEntity<TKey> : CoreEntity<TKey>
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
