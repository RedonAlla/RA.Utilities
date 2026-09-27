using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.EntityFramework;

/// <summary>
/// A generic repository that provides both read and write operations for an entity.
/// It inherits read behavior from <see cref="ReadRepositoryBase{T, TKey}"/> and delegates
/// write operations to an internal <see cref="WriteRepositoryBase{T, TKey}"/> instance.
/// </summary>
/// <typeparam name="T">Entity type (must inherit from <see cref="CoreEntity{TKey}"/>).</typeparam>
/// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
public class RepositoryBase<T, TKey> : ReadRepositoryBase<T, TKey>, IRepositoryBase<T, TKey>
    where T : CoreEntity<TKey>
    where TKey : notnull
{
    private readonly WriteRepositoryBase<T, TKey> _writeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T, TKey}"/> class.
    /// </summary>
    public RepositoryBase(DbContext dbContext) : base(dbContext)
    {
        _writeRepository = new WriteRepositoryBase<T, TKey>(dbContext);
    }

    #region IWriteRepositoryBase Implementation (delegated)

    /// <inheritdoc/>
    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        => _writeRepository.AddAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => _writeRepository.AddRangeAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        => _writeRepository.UpdateAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => _writeRepository.UpdateRangeAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public virtual Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        => _writeRepository.DeleteAsync(id, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        => _writeRepository.DeleteRangeAsync(ids, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _writeRepository.SaveChangesAsync(cancellationToken);

    #endregion
}
