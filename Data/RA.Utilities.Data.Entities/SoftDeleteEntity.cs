namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that support soft deletion, inheriting common properties from <see cref="WriteEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier.</typeparam>
public abstract class SoftDeleteEntity<TKey> : WriteEntity<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
