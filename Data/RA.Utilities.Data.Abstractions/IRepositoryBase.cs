using System;
using RA.Utilities.Data.Entities;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Defines a base interface for both read and write repository operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T> : IReadRepositoryBase<T>, IWriteRepositoryBase<T> where T : BaseEntity
{

}
