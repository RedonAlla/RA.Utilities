namespace RA.Utilities.Api.Sample.Models;

/// <summary>
/// A sample todo item.
/// </summary>
/// <param name="Id">The identifier of the todo.</param>
/// <param name="Title">The title of the todo.</param>
/// <param name="IsCompleted">Whether the todo is completed.</param>
internal sealed record Todo(int Id, string Title, bool IsCompleted);
