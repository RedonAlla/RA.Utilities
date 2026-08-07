using System;
using System.Security.Claims;
using FluentAssertions;
using RA.Utilities.Authorization.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Authorization.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    // =================================================================
    // GetUserId
    // =================================================================

    [Fact]
    public void GetUserId_WithValidGuid_ShouldReturnParsedGuid()
    {
        var id = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        }, "test"));

        Guid result = principal.GetUserId();
        result.Should().Be(id);
    }

    [Fact]
    public void GetUserId_WithoutNameIdentifierClaim_ShouldThrowInvalidOperationException()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity("test"));

        Action act = () => principal.GetUserId();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User id*");
    }

    [Fact]
    public void GetUserId_WithInvalidGuid_ShouldThrowInvalidOperationException()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"),
        }, "test"));

        Action act = () => principal.GetUserId();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetUserId_WithNullPrincipal_ShouldThrow()
    {
        ClaimsPrincipal? principal = null;
        // Null-forgiving operator is intentional — testing null propagation through the extension method
        Action act = () => principal!.GetUserId();
        act.Should().Throw<InvalidOperationException>();
    }

    // =================================================================
    // HasClaim
    // =================================================================

    [Fact]
    public void HasClaim_WithMatchingTypeAndValue_ShouldReturnTrue()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("permission", "CanDelete"),
        }, "test"));

        principal.HasClaim("permission", "CanDelete").Should().BeTrue();
    }

    [Fact]
    public void HasClaim_WithMatchingTypeButDifferentValue_ShouldReturnFalse()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("permission", "CanRead"),
        }, "test"));

        principal.HasClaim("permission", "CanDelete").Should().BeFalse();
    }

    [Fact]
    public void HasClaim_WithDifferentType_ShouldReturnFalse()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("other", "CanDelete"),
        }, "test"));

        principal.HasClaim("permission", "CanDelete").Should().BeFalse();
    }

    [Fact]
    public void HasClaim_WithNullPrincipal_ShouldReturnFalse()
    {
        ClaimsPrincipal? principal = null;
        // Use explicit extension call to avoid the built-in HasClaim instance method
        ClaimsPrincipalExtensions.HasClaim(principal, "permission", "CanDelete").Should().BeFalse();
    }

    // =================================================================
    // HasScope — space-separated OIDC standard
    // =================================================================

    [Fact]
    public void HasScope_WithSpaceSeparatedScopes_ShouldMatchIndividualScopes()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("scope", "api.read api.write openid profile"),
        }, "test"));

        principal.HasScope("api.read").Should().BeTrue();
        principal.HasScope("api.write").Should().BeTrue();
        principal.HasScope("openid").Should().BeTrue();
        principal.HasScope("profile").Should().BeTrue();
    }

    [Fact]
    public void HasScope_WithSingleScope_ShouldMatch()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("scope", "api.read"),
        }, "test"));

        principal.HasScope("api.read").Should().BeTrue();
    }

    [Fact]
    public void HasScope_WithNonMatchingScope_ShouldReturnFalse()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("scope", "api.read"),
        }, "test"));

        principal.HasScope("admin.access").Should().BeFalse();
    }

    [Fact]
    public void HasScope_IsCaseSensitive_ShouldReturnFalseOnCaseMismatch()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("scope", "api.read"),
        }, "test"));

        principal.HasScope("API.READ").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithoutScopeClaim_ShouldReturnFalse()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity("test"));
        principal.HasScope("api.read").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithNullPrincipal_ShouldReturnFalse()
    {
        ClaimsPrincipal? principal = null;
        // Use explicit extension call — the instance HasClaim method would shadow on null
        ClaimsPrincipalExtensions.HasScope(principal, "api.read").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithEmptyScopeValue_ShouldReturnFalse()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("scope", ""),
        }, "test"));

        principal.HasScope("api.read").Should().BeFalse();
    }
}
