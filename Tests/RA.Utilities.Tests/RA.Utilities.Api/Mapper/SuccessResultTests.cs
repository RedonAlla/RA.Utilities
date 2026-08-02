using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Mapper;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Tests.RA.Utilities.Api.Mapper;

/// <summary>
/// Contains unit tests for the <see cref="SuccessResult"/> class.
/// </summary>
public class SuccessResultTests
{
    // =================================================================
    // Ok Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Ok_WithPayload_ShouldReturn200WithSuccessResponse()
    {
        // Arrange
        var payload = new { Id = 1, Name = "Test" };

        // Act
        IResult result = SuccessResult.Ok(payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Success);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Ok_WithoutPayload_ShouldReturn200()
    {
        // Act
        IResult result = SuccessResult.Ok();

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Success);
    }

    // =================================================================
    // Created Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Created_WithoutParameters_ShouldReturn201()
    {
        // Act
        IResult result = SuccessResult.Created();

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Created);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Created_WithPayload_ShouldReturn201()
    {
        // Arrange
        var payload = new { Id = 1, Name = "Test" };

        // Act
        IResult result = SuccessResult.Created(payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Created);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Created_WithRouteAndPayload_ShouldReturn201()
    {
        // Arrange
        var payload = new { Id = 1, Name = "Test" };

        // Act
        IResult result = SuccessResult.Created("/products/1", payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Created);
    }

    // =================================================================
    // CreatedAtRoute Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void CreatedAtRoute_WithRouteNameValuesAndPayload_ShouldReturn201()
    {
        // Arrange
        var payload = new { Id = 1, Name = "Test" };

        // Act
        IResult result = SuccessResult.CreatedAtRoute("GetProduct", new { id = 1 }, payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Created);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void CreatedAtRoute_WithRouteNameAndValues_ShouldReturn201()
    {
        // Act
        IResult result = SuccessResult.CreatedAtRoute<object>("GetProduct", new { id = 1 });

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Created);
    }

    // =================================================================
    // Accepted Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Accepted_WithoutParameters_ShouldReturn202()
    {
        // Act
        IResult result = SuccessResult.Accepted();

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Accepted);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Accepted_WithRoute_ShouldReturn202()
    {
        // Act
        IResult result = SuccessResult.Accepted("/status/1");

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Accepted);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Accepted_WithPayload_ShouldReturn202()
    {
        // Arrange
        var payload = new { Status = "Processing" };

        // Act
        IResult result = SuccessResult.Accepted(payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Accepted);
    }

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Accepted_WithRouteAndPayload_ShouldReturn202()
    {
        // Arrange
        var payload = new { Status = "Processing" };

        // Act
        IResult result = SuccessResult.Accepted("/status/1", payload);

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.Accepted);
    }

    // =================================================================
    // NoContent Tests
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void NoContent_ShouldReturn204()
    {
        // Act
        IResult result = SuccessResult.NoContent();

        // Assert
        IStatusCodeHttpResult statusResult =
            result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        statusResult.StatusCode.Should().Be(BaseResponseCode.NoContent);
    }

    // =================================================================
    // Wrapping in SuccessResponse
    // =================================================================

    /// <summary>
    /// Tests the expected behavior for this scenario.
    /// </summary>
    [Fact]
    public void Ok_WithPayload_ShouldWrapInSuccessResponse()
    {
        // Arrange
        string payload = "test-value";

        // Act
        IResult result = SuccessResult.Ok(payload);

        // Assert
        IValueHttpResult valueResult =
            result.Should().BeAssignableTo<IValueHttpResult>().Subject;

        valueResult.Value.Should().BeAssignableTo<SuccessResponse<string>>();
    }
}
