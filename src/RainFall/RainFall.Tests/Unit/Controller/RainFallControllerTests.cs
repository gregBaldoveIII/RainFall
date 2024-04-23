using System.Net;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RainFall.API.Controllers;
using RainFall.Application.Queries;
using RainFall.Domain.Models;
using Xunit;

namespace RainFall.Tests.Unit.Controller
{
    public class RainFallControllerTests
    {
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly Mock<ILogger<RainFallController>> _mockLogger = new();

        [Fact]
        public async Task GetStationReading_ValidInput_ReturnsOkResult()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                         .ReturnsAsync(new RainfallReadingResponse { Readings = new List<RainfallReading>() });

            var controller = new RainFallController(_mockMediator.Object, _mockLogger.Object);

            var result = await controller.GetStationReading(123, 5, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeAssignableTo<List<RainfallReading>>()
                  .Which.Should().BeEmpty();
        }

        [Fact]
        public async Task GetStationReading_InvalidModelState_ReturnsBadRequest()
        {
            var controller = new RainFallController(_mockMediator.Object, _mockLogger.Object);
            controller.ModelState.AddModelError("count", "Count is required.");

            var result = await controller.GetStationReading(123, 5, CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetStationReading_NoReadings_ReturnsNotFoundResult()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                         .ReturnsAsync(new RainfallReadingResponse
                         {
                             ErrorDetail = new Error { StatusCode = HttpStatusCode.NotFound, Message = "Not Found" }
                         });

            var controller = new RainFallController(_mockMediator.Object, _mockLogger.Object);

            var result = await controller.GetStationReading(123, 5, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetStationReading_InternalServerError_ReturnsInternalServerErrorResult()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetRainfallReadingPerStationQuery>(), CancellationToken.None))
                         .ReturnsAsync(new RainfallReadingResponse
                         {
                             ErrorDetail = new Error { StatusCode = HttpStatusCode.InternalServerError, Message = "Internal Server Error" }
                         });

            var controller = new RainFallController(_mockMediator.Object, _mockLogger.Object);

            var result = await controller.GetStationReading(123, 5, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
