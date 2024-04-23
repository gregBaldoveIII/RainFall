using System.Net;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RainFall.Application.Service;
using RainFall.Domain.Constants;
using RainFall.Domain.Models;
using RainFall.Infrastructure.Interface;
using RainFall.Infrastructure.Models;
using Xunit;

namespace RainFall.Tests.Unit.Services
{
    public class RainFallReadingServiceTests
    {
        private readonly Mock<IEnvironmentAgencyAgent> _mockEnvironmentAgencyAgent = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<ILogger<RainFallReadingService>> _mockLogger = new();

        public RainFallReadingServiceTests()
        {
            _mockEnvironmentAgencyAgent.Setup(agent => agent.GetStationReading(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(new StationReadingSuccessResponse(new FloodMonitoringData()));
        }

        [Fact]
        public async Task GetRainfallReadingPerStation_CountBelowZero_ReturnsBadRequest()
        {
            var service = new RainFallReadingService(_mockEnvironmentAgencyAgent.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await service.GetRainfallReadingPerStation(123, -1);

            result.Should().NotBeNull();
            result.ErrorDetail.Should().NotBeNull();
            result.ErrorDetail.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetail.Message.Should().Be(ResponseMessage.Invalid);
        }

        [Fact]
        public async Task GetRainfallReadingPerStation_NoReadings_ReturnsNotFound()
        {
            var fixture = new Fixture();
            var floodMonitoringData = fixture.Build<FloodMonitoringData>()
                .With(x => x.Items, new List<Reading>())
                .Create();

            _mockEnvironmentAgencyAgent.Setup(agent => agent.GetStationReading(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(new StationReadingSuccessResponse(floodMonitoringData));

            var service = new RainFallReadingService(_mockEnvironmentAgencyAgent.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await service.GetRainfallReadingPerStation(123, 10);

            result.Should().NotBeNull();
            result.ErrorDetail.Should().NotBeNull();
            result.ErrorDetail.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.ErrorDetail.Message.Should().Be(ResponseMessage.NoReadings);
        }

        [Fact]
        public async Task GetRainfallReadingPerStation_Success_ReturnsReadings()
        {
            var fixture = new Fixture();

            var readings = fixture.Build<Reading>()
                .CreateMany(5)
                .ToList();

            var floodMonitoringData = fixture.Build<FloodMonitoringData>()
                .With(x => x.Items, readings)
                .Create();

            _mockEnvironmentAgencyAgent.Setup(agent => agent.GetStationReading(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(new StationReadingSuccessResponse(floodMonitoringData));

            _mockMapper.Setup(mapper => mapper.Map<RainfallReading>(readings))
                       .Returns<RainfallReading>(item => It.IsAny<RainfallReading>());

            var service = new RainFallReadingService(_mockEnvironmentAgencyAgent.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await service.GetRainfallReadingPerStation(123, 10);

            result.Should().NotBeNull();
            result.ErrorDetail.Message.Should().BeEmpty();
            result.Readings.Should().NotBeNull().And.HaveCount(readings.Count);
        }

        [Fact]
        public async Task GetRainfallReadingPerStation_ErrorResponse_ReturnsErrorResponse()
        {
            _mockEnvironmentAgencyAgent.Setup(agent => agent.GetStationReading(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(new StationReadingErrorResponse(HttpStatusCode.InternalServerError));

            var service = new RainFallReadingService(_mockEnvironmentAgencyAgent.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await service.GetRainfallReadingPerStation(123, 10);

            result.Should().NotBeNull();
            result.ErrorDetail.Should().NotBeNull();
            result.ErrorDetail.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.ErrorDetail.Message.Should().Be(ResponseMessage.ServiceError);
        }
    }
}
