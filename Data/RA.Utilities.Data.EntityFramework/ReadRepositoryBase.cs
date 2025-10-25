using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.EntityFramework;

/// <summary>
/// Provides a generic base implementation for read-only repository operations on entities using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public class ReadRepositoryBase<T> : IReadRepositoryBase<T>
    where T : BaseEntity
{
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// The database context used by the repository.
    /// </summary>
    public DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadRepositoryBase{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    public ReadRepositoryBase(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbSet = dbContext.Set<T>();
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        if (includeProperties != null && includeProperties.Length > 0)
        {
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
