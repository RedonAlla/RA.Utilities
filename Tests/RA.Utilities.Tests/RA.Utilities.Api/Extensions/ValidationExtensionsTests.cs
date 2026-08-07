using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.EndpointFilters;
using RA.Utilities.Api.Extensions;

namespace RA.Utilities.Tests.RA.Utilities.Api.Extensions;

/// <summary>
/// Contains unit tests for the <see cref="ValidationExtensions"/> class and <see cref="ValidationEndpointFilter{TModel}"/>.
/// </summary>
public class ValidationExtensionsTests
{
    // =================================================================
    // Test model and validator
    // =================================================================

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private sealed class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0.");
        }
    }

    // =================================================================
    // Validate<TModel> extension — chaining
    // =================================================================

    [Fact]
    public void Validate_ShouldReturnRouteHandlerBuilder_ForChaining()
    {
        // Arrange
        WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
        WebApplication app = builder.Build();

        // Act
        RouteHandlerBuilder chained = app.MapGet("/test", () => "test").Validate<TestModel>();

        // Assert
        chained.Should().NotBeNull();
        chained.Should().BeAssignableTo<RouteHandlerBuilder>();
    }

    // =================================================================
    // ValidationEndpointFilter — valid model
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithValidModel_ShouldCallNext()
    {
        // Arrange
        var validator = new TestModelValidator();
        var filter = new ValidationEndpointFilter<TestModel>([validator]);
        var model = new TestModel { Name = "John", Age = 25 };

        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, model);
        bool nextCalled = false;

        // Act
        _ = await filter.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(null);
        });

        // Assert
        nextCalled.Should().BeTrue();
    }

    // =================================================================
    // ValidationEndpointFilter — invalid model
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithInvalidModel_ShouldReturnErrorResult()
    {
        // Arrange
        var validator = new TestModelValidator();
        var filter = new ValidationEndpointFilter<TestModel>([validator]);
        var model = new TestModel { Name = "", Age = 0 };

        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, model);
        bool nextCalled = false;

        // Act
        object? result = await filter.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(null);
        });

        // Assert
        nextCalled.Should().BeFalse("invalid model should short-circuit before the handler");
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IStatusCodeHttpResult>();
        ((IStatusCodeHttpResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    // =================================================================
    // ValidationEndpointFilter — no validators registered
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithNoValidators_ShouldCallNext()
    {
        // Arrange
        var filter = new ValidationEndpointFilter<TestModel>([]);
        var model = new TestModel { Name = "", Age = 0 };

        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, model);
        bool nextCalled = false;

        // Act
        _ = await filter.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(null);
        });

        // Assert
        nextCalled.Should().BeTrue("no validators means validation is skipped");
    }

    // =================================================================
    // ValidationEndpointFilter — null model argument
    // =================================================================

    [Fact]
    public async Task InvokeAsync_WithNullModelArgument_ShouldCallNext()
    {
        // Arrange
        var validator = new TestModelValidator();
        var filter = new ValidationEndpointFilter<TestModel>([validator]);

        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext);
        bool nextCalled = false;

        // Act
        _ = await filter.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(null);
        });

        // Assert
        nextCalled.Should().BeTrue("null model should pass through without validation");
    }
}
