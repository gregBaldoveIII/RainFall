using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RainFall.API.Controllers;
using RainFall.Application.Interface;
using RainFall.Application.Queries;
using RainFall.Domain.Models;
using Xunit;

namespace RainFall.Tests.Unit.Controller;

public class RainFallControllerTests
{
    private readonly Mock<IQueryHandler<GetRainfallReadingPerStationQuery, RainfallReadingResponse>> _mockHandler = new();
    private readonly Mock<ILogger<RainFallController>> _mockLogger = new();

    [Fact]
    public async Task GetStationReading_ValidInput_ReturnsOkResult()
    {
        _mockHandler.Setup(handler => handler.HandleAsync(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                    .ReturnsAsync(new RainfallReadingResponse { Readings = new List<RainfallReading>() });

        var controller = new RainFallController(_mockHandler.Object, _mockLogger.Object);

        var result = await controller.GetStationReading(123, 5, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeAssignableTo<List<RainfallReading>>()
              .Which.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStationReading_InvalidModelState_ReturnsBadRequest()
    {
        var controller = new RainFallController(_mockHandler.Object, _mockLogger.Object);
        controller.ModelState.AddModelError("count", "Count is required.");

        var result = await controller.GetStationReading(123, 5, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetStationReading_NoReadings_ReturnsNotFoundResult()
    {
        _mockHandler.Setup(handler => handler.HandleAsync(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                    .ReturnsAsync(new RainfallReadingResponse
                    {
                        ErrorDetail = new Error { StatusCode = HttpStatusCode.NotFound, Message = "Not Found" }
                    });

        var controller = new RainFallController(_mockHandler.Object, _mockLogger.Object);

        var result = await controller.GetStationReading(123, 5, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetStationReading_InternalServerError_ReturnsInternalServerErrorResult()
    {
        _mockHandler.Setup(handler => handler.HandleAsync(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                    .ReturnsAsync(new RainfallReadingResponse
                    {
                        ErrorDetail = new Error { StatusCode = HttpStatusCode.InternalServerError, Message = "Internal Server Error" }
                    });

        var controller = new RainFallController(_mockHandler.Object, _mockLogger.Object);

        var result = await controller.GetStationReading(123, 5, CancellationToken.None);

        result.Should().BeOfType<ObjectResult>()
              .Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}

