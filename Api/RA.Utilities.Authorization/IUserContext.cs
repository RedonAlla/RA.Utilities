using System;
using System.Collections.Generic;

namespace RA.Utilities.Authorization;

/// <summary>
/// Provides a strongly-typed way to access the claims of the currently authenticated user.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the user's unique identifier from the NameIdentifier claim
    /// (mapped from the 'sub' claim by the JWT middleware).
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    string? Id { get; }

    /// <summary>
    /// Gets the user's unique identifier as a <see cref="Guid"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user is not authenticated or the NameIdentifier claim is missing or not a valid Guid.
    /// </exception>
    Guid UserId { get; }

    /// <summary>
    /// Gets the user's email address from the email claim.
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the user's name from the name claim.
    /// Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the value of the first claim with the specified type.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>The value of the first claim of the specified type, or null if not found.</returns>
    string? GetClaimValue(string claimType);

    /// <summary>
    /// Gets all values for a specific claim type.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>An enumerable of strings containing the values of the claims, or an empty enumerable if not found.</returns>
    IEnumerable<string> GetClaimValues(string claimType);

    /// <summary>
    /// Checks if the current user has a claim with the specified type and value.
    /// </summary>
    /// <param name="claimType">The type of the claim to check.</param>
    /// <param name="claimValue">The value of the claim to check for.</param>
    /// <returns><c>true</c> if the user has a matching claim; otherwise, <c>false</c>.</returns>
    bool HasClaim(string claimType, string claimValue);

    /// <summary>
    /// Checks if the current user has the specified OAuth 2.0 / OIDC scope.
    /// Handles both space-separated and individual scope claim entries.
    /// </summary>
    /// <param name="scopeValue">The scope value to check for.</param>
    /// <returns><c>true</c> if the user has the specified scope; otherwise, <c>false</c>.</returns>
    bool HasScope(string scopeValue);

    /// <summary>
    /// Checks if the current user is a member of the specified role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns><c>true</c> if the user is in the specified role; otherwise, <c>false</c>.</returns>
    bool IsInRole(string roleName);
}
