using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Defines a base interface for both read and write repository operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
public interface IRepositoryBase<T, TKey> : IReadRepositoryBase<T, TKey>, IWriteRepositoryBase<T, TKey> where T : CoreEntity<TKey> where TKey : notnull;
