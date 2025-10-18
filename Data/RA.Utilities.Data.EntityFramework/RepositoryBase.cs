using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Data.Abstractions;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.EntityFramework;

/// <summary>
/// RepositoryBase combines read and write capabilities by reusing ReadRepositoryBase and WriteRepositoryBase.
/// It inherits read behavior and delegates write operations to an internal write base instance.
/// A generic repository that provides both read and write operations for an entity.
/// It inherits from <see cref="WriteRepositoryBase{T}"/>, which provides the write logic,
/// which in turn inherits from <see cref="ReadRepositoryBase{T}"/> for read logic.
/// </summary>
/// <typeparam name="T">Entity type (must inherit BaseEntity).</typeparam>
public class RepositoryBase<T> : ReadRepositoryBase<T>, IRepositoryBase<T>
    where T : BaseEntity
{
    private readonly WriteRepositoryBase<T> _writeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
    /// </summary>
    public RepositoryBase(DbContext dbContext) : base(dbContext)
    {
        _writeRepository = new WriteRepositoryBase<T>(dbContext);
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
    public virtual Task DeleteAsync<TId>(TId entity, CancellationToken cancellationToken = default) where TId : notnull
        => _writeRepository.DeleteAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> DeleteRangeAsync(List<Guid> entities, CancellationToken cancellationToken = default)
        => _writeRepository.DeleteRangeAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _writeRepository.SaveChangesAsync(cancellationToken);

    #endregion
}
