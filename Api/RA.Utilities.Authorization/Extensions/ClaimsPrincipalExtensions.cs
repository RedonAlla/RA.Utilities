using System;
using System.Linq;
using System.Security.Claims;

namespace RA.Utilities.Authorization.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user's unique identifier as a <see cref="Guid"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> from which to retrieve the user ID.</param>
    /// <returns>The user's <see cref="Guid"/> identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user ID claim is not present or cannot be parsed as a Guid.</exception>
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new InvalidOperationException("User id is unavailable");
    }

    /// <summary>
    /// Checks if the principal has a claim with the specified type and value.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to check.</param>
    /// <param name="claimType">The type of the claim to check.</param>
    /// <param name="claimValue">The value of the claim to check for.</param>
    /// <returns><c>true</c> if the principal has a matching claim; otherwise, <c>false</c>.</returns>
    public static bool HasClaim(this ClaimsPrincipal? principal, string claimType, string claimValue) =>
        principal?.HasClaim(c => c.Type == claimType && c.Value == claimValue) ?? false;

    /// <summary>
    /// Checks if the principal has the specified OAuth 2.0 / OIDC scope.
    /// Handles both space-separated and individual scope claim entries.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to check.</param>
    /// <param name="scopeValue">The scope value to check for.</param>
    /// <returns><c>true</c> if the principal has the specified scope; otherwise, <c>false</c>.</returns>
    public static bool HasScope(this ClaimsPrincipal? principal, string scopeValue)
    {
        Claim? scopeClaim = principal?.FindFirst("scope");
        if (scopeClaim is null)
            return false;

        return scopeClaim.Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains(scopeValue, StringComparer.Ordinal);
    }
}
