using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RA.Utilities.Authorization;

/// <summary>
/// Provides a strongly-typed way to access the claims of the currently authenticated user.
/// This service is typically registered as Transient or Scoped and relies on IHttpContextAccessor
/// to get the user information for the current request.
/// </summary>
public class AppUser
{
    private readonly ClaimsPrincipal? User;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor, injected by the DI container.</param>
    public AppUser(IHttpContextAccessor httpContextAccessor)
    {
        User = httpContextAccessor.HttpContext?.User;
    }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the user's unique identifier (from the 'sub' or 'nameidentifier' claim).
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Id => GetClaimValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// Gets the user's email address (from the 'email' claim).
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Email => GetClaimValue(ClaimTypes.Email);

    /// <summary>
    /// Gets the user's name (from the 'name' claim).
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Name => GetClaimValue(ClaimTypes.Name);

    /// <summary>
    /// Checks if the current user is a member of the specified role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the user is in the specified role; otherwise, false.</returns>
    public bool IsInRole(string roleName) => User?.IsInRole(roleName) ?? false;

    /// <summary>
    /// Checks if the current user has a specific claim with a given type and value.
    /// </summary>
    /// <param name="claimValue">The value of the claim to check for.</param>
    /// <returns><c>true</c> if the current user has the specified claim; otherwise, <c>false</c>.</returns>
    public bool HasClaim(string claimValue) =>
        User?.HasClaim(c => c.Type == "claim" && c.Value == claimValue) ?? false;

    /// <summary>
    /// Checks if the current user has a specific scope value.
    /// </summary>
    /// <param name="scopeValue">The scope value to check for.</param>
    /// <returns><c>true</c> if the current user has the specified scope; otherwise, <c>false</c>.</returns>
    public bool HasScope(string scopeValue) =>
        User?.HasClaim(c => c.Type == "scope" && c.Value == scopeValue) ?? false;

    /// <summary>
    /// Checks if the current user has a specific claim.
    /// </summary>
    /// <param name="claimValue">The claim value to check for.</param>
    /// <returns>The value of the first claim of the specified type, or null if not found.</returns>
    public string? GetClaimValue(string claimValue) => User?.FindFirst(claimValue)?.Value;

    /// <summary>
    /// Gets all values for a specific claim type.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>An enumerable of strings containing the values of the claims, or an empty enumerable if not found.</returns>
    public IEnumerable<string> GetClaimValues(string claimType) =>
        User?.FindAll(claimType).Select(c => c.Value) ?? [];
}
