using System;
using System.Security.Claims;

namespace RA.Utilities.Authorization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the value for the first claim of the specified type, otherwise null if the claim is not present.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
    /// <param name="claimType">The claim type whose first value should be returned.</param>
    /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>
    public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
    {
        ArgumentNullException.ThrowIfNull(principal);
        Claim? claim = principal.FindFirst(claimType);
        return claim?.Value;
    }

    /// <summary>
    /// Gets the user's unique identifier as a <see cref="Guid"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> from which to retrieve the user ID.</param>
    /// <returns>The user's <see cref="Guid"/> identifier.</returns>
    /// <exception cref="ApplicationException">Thrown if the user ID claim is not present or cannot be parsed as a Guid.</exception>
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }

    /// <summary>
    /// Checks if the principal has a specific claim with a given type and value.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to check.</param>
    /// <param name="claimValue">The value of the claim to check for.</param>
    /// <returns><c>true</c> if the principal has the specified claim; otherwise, <c>false</c>.</returns>
    public static bool HasClaim(this ClaimsPrincipal? principal, string claimValue) =>
        principal?.HasClaim(c => c.Type == "claim" && c.Value == claimValue) ?? false;

    /// <summary>
    /// Checks if the principal has a specific scope claim.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to check.</param>
    /// <param name="scopeValue">The scope value to check for.</param>
    /// <returns><c>true</c> if the principal has the specified scope; otherwise, <c>false</c>.</returns>
    public static bool HasScope(this ClaimsPrincipal? principal, string scopeValue) =>
        principal?.HasClaim(c => c.Type == "scope" && c.Value == scopeValue) ?? false;
}
