using System.Threading;
using System.Threading.Tasks;

namespace RA.Utilities.Data.Abstractions;

/// <summary>
/// Represents a generic interface for a database context.
/// </summary>
public interface IDbContext
{
    /// <summary>
    ///     Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous save operation.
    ///     The task result contains the number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
