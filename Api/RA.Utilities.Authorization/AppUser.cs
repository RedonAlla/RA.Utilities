using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Authorization.Extensions;

namespace RA.Utilities.Authorization;

/// <summary>
/// Provides a strongly-typed way to access the claims of the currently authenticated user.
/// This service is registered as Transient and relies on <see cref="IHttpContextAccessor"/>
/// to get the user information for the current request.
/// </summary>
public class AppUser
{
    private readonly ClaimsPrincipal? _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor, injected by the DI container.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpContextAccessor"/> is null.</exception>
    public AppUser(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _user = httpContextAccessor.HttpContext?.User;
    }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the user's unique identifier from the NameIdentifier claim
    /// (mapped from the 'sub' claim by the JWT middleware).
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Id => GetClaimValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// Gets the user's unique identifier as a <see cref="Guid"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user is not authenticated or the NameIdentifier claim is missing or not a valid Guid.
    /// </exception>
    public Guid UserId => _user.GetUserId();

    /// <summary>
    /// Gets the user's email address from the email claim.
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Email => GetClaimValue(ClaimTypes.Email);

    /// <summary>
    /// Gets the user's name from the name claim.
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    public string? Name => GetClaimValue(ClaimTypes.Name);

    /// <summary>
    /// Checks if the current user is a member of the specified role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns><c>true</c> if the user is in the specified role; otherwise, <c>false</c>.</returns>
    public bool IsInRole(string roleName) => _user?.IsInRole(roleName) ?? false;

    /// <summary>
    /// Checks if the current user has a claim with the specified type and value.
    /// </summary>
    /// <param name="claimType">The type of the claim to check.</param>
    /// <param name="claimValue">The value of the claim to check for.</param>
    /// <returns><c>true</c> if the user has a matching claim; otherwise, <c>false</c>.</returns>
    public bool HasClaim(string claimType, string claimValue) =>
        _user?.HasClaim(c => c.Type == claimType && c.Value == claimValue) ?? false;

    /// <summary>
    /// Checks if the current user has the specified OAuth 2.0 / OIDC scope.
    /// Handles both space-separated and individual scope claim entries.
    /// </summary>
    /// <param name="scopeValue">The scope value to check for.</param>
    /// <returns><c>true</c> if the user has the specified scope; otherwise, <c>false</c>.</returns>
    public bool HasScope(string scopeValue)
    {
        Claim? scopeClaim = _user?.FindFirst("scope");
        if (scopeClaim is null)
            return false;

        // Standard OIDC: scopes are space-separated in a single claim value.
        // Also supports individual scope-per-claim entries for non-standard setups.
        return scopeClaim.Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains(scopeValue, StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets the value of the first claim with the specified type.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>The value of the first claim of the specified type, or null if not found.</returns>
    public string? GetClaimValue(string claimType) => _user?.FindFirst(claimType)?.Value;

    /// <summary>
    /// Gets all values for a specific claim type.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>An enumerable of strings containing the values of the claims, or an empty enumerable if not found.</returns>
    public IEnumerable<string> GetClaimValues(string claimType) =>
        _user?.FindAll(claimType).Select(c => c.Value) ?? [];
}
