using System;

namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that track creation and modification user identifiers,
/// inheriting common properties from <see cref="WriteEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class AuditableBaseEntity<TKey> : WriteEntity<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
