using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Authorization;

namespace RA.Utilities.Tests.RA.Utilities.Authorization;

public class AppUserTests
{
    private static AppUser CreateAppUser(ClaimsPrincipal? user)
    {
        var accessor = new MockHttpContextAccessor(user);
        return new AppUser(accessor);
    }

    // =================================================================
    // Constructor — null guard
    // =================================================================

    [Fact]
    public void Constructor_WithNullHttpContextAccessor_ShouldThrowArgumentNullException()
    {
        Action act = () => _ = new AppUser(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpContextAccessor");
    }

    [Fact]
    public void Constructor_WithNullHttpContext_ShouldNotThrow()
    {
        var accessor = new MockHttpContextAccessor(null, hasNullContext: true);
        Action act = () => _ = new AppUser(accessor);
        act.Should().NotThrow();
    }

    // =================================================================
    // IsAuthenticated
    // =================================================================

    [Fact]
    public void IsAuthenticated_WithAuthenticatedUser_ShouldReturnTrue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_WithUnauthenticatedUser_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: false));
        user.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_WithNullUser_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(null);
        user.IsAuthenticated.Should().BeFalse();
    }

    // =================================================================
    // Id (string?)
    // =================================================================

    [Fact]
    public void Id_WithNameIdentifierClaim_ShouldReturnClaimValue()
    {
        string id = Guid.NewGuid().ToString();
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.NameIdentifier, id)));
        user.Id.Should().Be(id);
    }

    [Fact]
    public void Id_WithoutNameIdentifierClaim_ShouldReturnNull()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.Id.Should().BeNull();
    }

    [Fact]
    public void Id_WithNullUser_ShouldReturnNull()
    {
        AppUser user = CreateAppUser(null);
        user.Id.Should().BeNull();
    }

    // =================================================================
    // UserId (Guid)
    // =================================================================

    [Fact]
    public void UserId_WithValidGuid_ShouldReturnParsedGuid()
    {
        var id = Guid.NewGuid();
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.NameIdentifier, id.ToString())));
        user.UserId.Should().Be(id);
    }

    [Fact]
    public void UserId_WithoutNameIdentifierClaim_ShouldThrowInvalidOperationException()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        Action act = () => _ = user.UserId;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User id*");
    }

    [Fact]
    public void UserId_WithInvalidGuid_ShouldThrowInvalidOperationException()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid")));
        Action act = () => _ = user.UserId;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UserId_WithNullUser_ShouldThrowInvalidOperationException()
    {
        AppUser user = CreateAppUser(null);
        Action act = () => _ = user.UserId;
        act.Should().Throw<InvalidOperationException>();
    }

    // =================================================================
    // Name
    // =================================================================

    [Fact]
    public void Name_WithNameClaim_ShouldReturnClaimValue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.Name, "John Doe")));
        user.Name.Should().Be("John Doe");
    }

    [Fact]
    public void Name_WithoutNameClaim_ShouldReturnNull()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.Name.Should().BeNull();
    }

    // =================================================================
    // Email
    // =================================================================

    [Fact]
    public void Email_WithEmailClaim_ShouldReturnClaimValue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.Email, "john@example.com")));
        user.Email.Should().Be("john@example.com");
    }

    [Fact]
    public void Email_WithoutEmailClaim_ShouldReturnNull()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.Email.Should().BeNull();
    }

    // =================================================================
    // IsInRole
    // =================================================================

    [Fact]
    public void IsInRole_WithMatchingRole_ShouldReturnTrue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.Role, "Admin")));
        user.IsInRole("Admin").Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WithNonMatchingRole_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim(ClaimTypes.Role, "User")));
        user.IsInRole("Admin").Should().BeFalse();
    }

    [Fact]
    public void IsInRole_WithNullUser_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(null);
        user.IsInRole("Admin").Should().BeFalse();
    }

    // =================================================================
    // HasClaim
    // =================================================================

    [Fact]
    public void HasClaim_WithMatchingClaim_ShouldReturnTrue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("permission", "CanDelete")));
        user.HasClaim("permission", "CanDelete").Should().BeTrue();
    }

    [Fact]
    public void HasClaim_WithNonMatchingValue_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("permission", "CanRead")));
        user.HasClaim("permission", "CanDelete").Should().BeFalse();
    }

    [Fact]
    public void HasClaim_WithNonMatchingType_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("other_claim", "CanDelete")));
        user.HasClaim("permission", "CanDelete").Should().BeFalse();
    }

    [Fact]
    public void HasClaim_WithNullUser_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(null);
        user.HasClaim("permission", "CanDelete").Should().BeFalse();
    }

    // =================================================================
    // HasScope — space-separated
    // =================================================================

    [Fact]
    public void HasScope_WithSpaceSeparatedScopes_ShouldMatch()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("scope", "api.read api.write openid profile")));
        user.HasScope("api.read").Should().BeTrue();
        user.HasScope("api.write").Should().BeTrue();
    }

    [Fact]
    public void HasScope_WithNonMatchingScope_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("scope", "api.read")));
        user.HasScope("admin.access").Should().BeFalse();
    }

    [Fact]
    public void HasScope_IsCaseSensitive_ShouldReturnFalseOnCaseMismatch()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("scope", "api.read")));
        user.HasScope("API.READ").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithoutScopeClaim_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.HasScope("api.read").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithNullUser_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(null);
        user.HasScope("api.read").Should().BeFalse();
    }

    [Fact]
    public void HasScope_WithEmptyScopeValue_ShouldReturnFalse()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("scope", "")));
        user.HasScope("api.read").Should().BeFalse();
    }

    // =================================================================
    // GetClaimValue
    // =================================================================

    [Fact]
    public void GetClaimValue_WithExistingClaimType_ShouldReturnValue()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("custom_claim", "custom_value")));
        user.GetClaimValue("custom_claim").Should().Be("custom_value");
    }

    [Fact]
    public void GetClaimValue_WithNonExistingClaimType_ShouldReturnNull()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.GetClaimValue("missing_claim").Should().BeNull();
    }

    // =================================================================
    // GetClaimValues
    // =================================================================

    private static readonly string[] ExpectedRoles = ["Admin", "User"];

    [Fact]
    public void GetClaimValues_WithMultipleMatchingClaims_ShouldReturnAllValues()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true,
            new Claim("role", "Admin"),
            new Claim("role", "User")));

        IEnumerable<string> roles = user.GetClaimValues("role");
        roles.Should().BeEquivalentTo(ExpectedRoles);
    }

    [Fact]
    public void GetClaimValues_WithoutMatchingClaims_ShouldReturnEmpty()
    {
        AppUser user = CreateAppUser(CreatePrincipal(authenticated: true));
        user.GetClaimValues("role").Should().BeEmpty();
    }

    [Fact]
    public void GetClaimValues_WithNullUser_ShouldReturnEmpty()
    {
        AppUser user = CreateAppUser(null);
        user.GetClaimValues("role").Should().BeEmpty();
    }

    // =================================================================
    // Helpers
    // =================================================================

    private static ClaimsPrincipal CreatePrincipal(bool authenticated, params Claim[] claims)
    {
        var identity = new ClaimsIdentity(
            claims,
            authenticated ? "test" : null);
        return new ClaimsPrincipal(identity);
    }

    private sealed class MockHttpContextAccessor : IHttpContextAccessor
    {
        public MockHttpContextAccessor(ClaimsPrincipal? user, bool hasNullContext = false)
        {
            if (!hasNullContext)
            {
                HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() };
            }
        }

        public HttpContext? HttpContext { get; set; }
    }
}
