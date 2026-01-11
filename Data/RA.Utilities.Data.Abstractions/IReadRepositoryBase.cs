using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Defines a base interface for read-only repository operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IReadRepositoryBase<T> : IRepository where T : notnull, CoreEntity
{
    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <typeparam name="TId">The type of the entity's ID.</typeparam>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the entity if found, otherwise <see langword="null"/>.</returns>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>
    /// Lists entities of type <typeparamref name="T"/> asynchronously based on the provided criteria.
    /// </summary>
    /// <param name="filter">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="take">The number of elements to return.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="includeProperties">An array of navigation properties to include.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a read-only list of entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);
}
