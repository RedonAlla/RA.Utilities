namespace RA.Utilities.Data.Entities;

/// <summary>
/// Base class for entities that need to track creation and modification users,
/// inheriting common properties from <see cref="BaseEntity"/>.
/// </summary>
public abstract class AuditableBaseEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string LastModifiedBy { get; set; }
}
