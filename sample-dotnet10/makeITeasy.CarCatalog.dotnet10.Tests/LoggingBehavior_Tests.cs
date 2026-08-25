using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class DestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
        {
            if (value is Car car)
            {
                result = propertyValueFactory.CreatePropertyValue(
                    new { car.Name, car.ReleaseYear, car.Brand },
                    destructureObjects: true);
                return true;
            }

            result = null;
            return false;
        }
    }

    public class LoggingBehavior_Tests
    {
        [Fact]
        public async Task Handle_LogsRequestAndResponse_WithMockedLogger()
        {
            MediatRLog mediatRLog = new();

            var loggerMock = new Mock<ILogger<LoggingBehavior<CreateEntityCommand<Car>, CommandResult<Car>>>>();
            var behavior = new LoggingBehavior<CreateEntityCommand<Car>, CommandResult<Car>>(loggerMock.Object, mediatRLog);

            CreateEntityCommand<Car> command = new (new Car { Name = "TestCar" });
            CommandResult<Car> expectedResponse = new (CommandState.Success) { Entity = new Car { Id = 1, Name = "TestCar" } };

            Task<CommandResult<Car>> next(CancellationToken _ = default) => Task.FromResult(expectedResponse);

            CommandResult<Car> response = await behavior.Handle(command, next, TestContext.Current.CancellationToken);

            response.Result.Should().Be(CommandState.Success);

            mediatRLog.Counter.Should().Be(2);
            mediatRLog.Logs.Should().HaveCount(2);
            mediatRLog.Logs[0].Should().StartWith("Processing").And.Contain("TestCar");
            mediatRLog.Logs[1].Should().StartWith("Processed");

            loggerMock.Verify(
                x => x.Log(
                    logLevel: LogLevel.Information,
                    eventId: It.IsAny<EventId>(),
                    state: It.Is<It.IsAnyType>((_, __) => true),
                    exception: It.IsAny<Exception?>(),
                    formatter: It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
        }

        public class PlainRequest : IRequest<string>
        {
        }

        [Fact]
        public async Task Handle_WithNonCommandRequest_DoesNotLog()
        {
            MediatRLog mediatRLog = new();

            var loggerMock = new Mock<ILogger<LoggingBehavior<PlainRequest, string>>>();
            var behavior = new LoggingBehavior<PlainRequest, string>(loggerMock.Object, mediatRLog);

            RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

            string response = await behavior.Handle(new PlainRequest(), next, TestContext.Current.CancellationToken);

            response.Should().Be("ok");
            mediatRLog.Counter.Should().Be(0);
            mediatRLog.Logs.Should().BeEmpty();

            loggerMock.Verify(
                x => x.Log(
                    logLevel: It.IsAny<LogLevel>(),
                    eventId: It.IsAny<EventId>(),
                    state: It.Is<It.IsAnyType>((_, __) => true),
                    exception: It.IsAny<Exception?>(),
                    formatter: It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        private sealed class CollectingSink(List<LogEvent> events) : ILogEventSink
        {
            public void Emit(LogEvent logEvent) => events.Add(logEvent);
        }

        [Fact]
        public void DestructuringPolicy_Should_Hide_Unwanted_Properties()
        {
            List<LogEvent> events = [];

            var logger = new LoggerConfiguration()
                .Destructure.With(new DestructuringPolicy())
                .WriteTo.Sink(new CollectingSink(events))
                .CreateLogger();

            Car car = new()
            {
                Id = 42,
                Name = "TestCar",
                ReleaseYear = 2020,
                Brand = new Brand { Name = "TestBrand" },
                CarType = CarType.Break
            };

            logger.Information("Car logged : {@Car}", car);

            events.Should().ContainSingle();

            LogEvent logEvent = events[0];
            logEvent.Properties.Should().ContainKey("Car");

            StructureValue structure = logEvent.Properties["Car"].Should().BeOfType<StructureValue>().Subject;

            List<string> projectedNames = [.. structure.Properties.Select(p => p.Name)];

            projectedNames.Should().BeEquivalentTo(nameof(Car.Name), nameof(Car.ReleaseYear), nameof(Car.Brand));

            projectedNames.Should().NotContain(nameof(Car.Id));
            projectedNames.Should().NotContain(nameof(Car.CarType));

            structure.Properties.Single(p => p.Name == nameof(Car.Name)).Value
                .Should().BeOfType<ScalarValue>().Which.Value.Should().Be("TestCar");
            structure.Properties.Single(p => p.Name == nameof(car.ReleaseYear)).Value
                .Should().BeOfType<ScalarValue>().Which.Value.Should().Be(2020);
            structure.Properties.SingleOrDefault(p => p.Name == nameof(Car.Id)).Should().BeNull();

            StructureValue brandStructure = structure.Properties.Single(p => p.Name == "Brand").Value
                .Should().BeOfType<StructureValue>().Subject;

            brandStructure.Properties.Single(p => p.Name == nameof(Brand.Name)).Value
                .Should().BeOfType<ScalarValue>().Which.Value.Should().Be("TestBrand");
        }
    }
}
