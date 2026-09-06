namespace RA.Utilities.Api.Sample.Models;

/// <summary>
/// A sample user.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
internal sealed record User(int Id, string Name);
