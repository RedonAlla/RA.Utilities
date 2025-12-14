using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.EntityFramework;

/// <summary>
/// Provides a generic base implementation for write-only repository operations on entities using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public class WriteRepositoryBase<T> : IWriteRepositoryBase<T>
    where T : CoreEntity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteRepositoryBase{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    public WriteRepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public virtual async Task<int> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities.Count();
    }

    /// <inheritdoc />
    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        _dbSet.Update(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public virtual Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.FromResult(entities.Count());
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        _ = await _dbSet.Where(e => e.Id.Equals(id)).ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> DeleteRangeAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        _ = await _dbSet.Where(e => ids.Contains(e.Id)).ExecuteDeleteAsync(cancellationToken);
        return ids.Count;
    }

    /// <inheritdoc />
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
