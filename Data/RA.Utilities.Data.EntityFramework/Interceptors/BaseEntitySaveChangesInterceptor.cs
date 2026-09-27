using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.EntityFramework.Interceptors;

/// <summary>
/// Intercepts SaveChanges operations to automatically update timestamp properties like
/// <see cref="BaseEntity{TKey}.CreatedAt"/> and <see cref="WriteEntity{TKey}.LastModifiedAt"/>
/// on entities derived from <see cref="BaseEntity{TKey}"/> or <see cref="WriteEntity{TKey}"/>.
/// </summary>
public class BaseEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private const string CreatedAtProperty = nameof(BaseEntity<>.CreatedAt);
    private const string LastModifiedAtProperty = nameof(WriteEntity<>.LastModifiedAt);

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {

            if (entry.State is EntityState.Added && entry.Metadata.FindProperty(CreatedAtProperty) is not null)
            {
                entry.Property(CreatedAtProperty).CurrentValue = DateTime.UtcNow;
            }

            if (entry.State is EntityState.Modified && entry.Metadata.FindProperty(LastModifiedAtProperty) is not null)
            {
                entry.Property(LastModifiedAtProperty).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
