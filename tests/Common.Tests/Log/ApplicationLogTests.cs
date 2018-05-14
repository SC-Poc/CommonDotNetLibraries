using System;
using Lykke.Common.Log;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;
using Xunit;

namespace Common.Tests.Log
{
    public class ApplicationLogTests
    {
        [Fact]
        public void All_parameters_passed_correctly()
        {
            // Arrange

            var loggerMock = new Mock<ILogger>();
            var log = ApplicationLog.Get(loggerMock.Object, "Lykke.Common.Tests", "1.2.3", "env-info");

            // Act

            var componentLog0 = log.GetComponentLog<ApplicationLogTests>();
            var componentLog1 = log.GetComponentLog("ApplicationLogTests1");
            var componentLog2 = log.GetEmptyComponentLog();

            var processLog0 = log.GetProcessLog<ApplicationLogTests>("process0");
            var processLog2 = log.GetProcessLog("ApplicationLogTests2", "process2");
            var processLog3 = log.GetEmptyProcessLog();

            log.Trace("component1", "process1", "message1", "context1", new DateTime(2018, 05, 10));
            log.Debug("component2", "process2", "message2", "context2", new DateTime(2018, 05, 11));
            log.Info("component3", "process3", "message3", "context3", new DateTime(2018, 05, 12));
            log.Warning("component4", "process4", "message4", "context4", new InvalidOperationException("exception4"), new DateTime(2018, 05, 13));
            log.Error("component5", "process5", "message5", "context5", new InvalidOperationException("exception5"), new DateTime(2018, 05, 14));
            log.FatalError("component6", "process6", "message6", "context6", new InvalidOperationException("exception6"), new DateTime(2018, 05, 15));
            log.Monitor("message7", "context7", new DateTime(2018, 05, 16));

            // Assert

            Assert.Equal("Lykke.Common.Tests", log.AppName);
            Assert.Equal("1.2.3", log.AppVersion);
            Assert.Equal("env-info", log.EnvInfo);

            Assert.Equal("ApplicationLogTests", componentLog0.Component);
            Assert.Equal("ApplicationLogTests1", componentLog1.Component);
            Assert.Null(componentLog2.Component);

            Assert.Equal("process0", processLog0.Process);
            Assert.Equal("process2", processLog2.Process);
            Assert.Null(processLog3.Process);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component1"),
                It.Is<string>(s => s == "process1"),
                It.Is<LogLevel>(l => l == LogLevel.Trace),
                It.Is<string>(s => s == "message1"),
                It.Is<string>(s => s == "context1"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 10))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component2"),
                It.Is<string>(s => s == "process2"),
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.Is<string>(s => s == "message2"),
                It.Is<string>(s => s == "context2"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 11))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component3"),
                It.Is<string>(s => s == "process3"),
                It.Is<LogLevel>(l => l == LogLevel.Info),
                It.Is<string>(s => s == "message3"),
                It.Is<string>(s => s == "context3"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 12))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component4"),
                It.Is<string>(s => s == "process4"),
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.Is<string>(s => s == "message4"),
                It.Is<string>(s => s == "context4"),
                It.Is<InvalidOperationException>(e => e.Message == "exception4"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 13))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component5"),
                It.Is<string>(s => s == "process5"),
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.Is<string>(s => s == "message5"),
                It.Is<string>(s => s == "context5"),
                It.Is<InvalidOperationException>(e => e.Message == "exception5"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 14))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component6"),
                It.Is<string>(s => s == "process6"),
                It.Is<LogLevel>(l => l == LogLevel.FatalError),
                It.Is<string>(s => s == "message6"),
                It.Is<string>(s => s == "context6"),
                It.Is<InvalidOperationException>(e => e.Message == "exception6"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 15))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == null),
                It.Is<string>(s => s == null),
                It.Is<LogLevel>(l => l == LogLevel.Monitor),
                It.Is<string>(s => s == "message7"),
                It.Is<string>(s => s == "context7"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 16))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<LogLevel>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Exception>(),
                It.IsAny<DateTime>()), Times.Exactly(7));
        }

        [Fact]
        public void Executing_application_parameters_catched_correctly()
        {
            // Arrange

            var loggerMock = new Mock<ILogger>();
            
            Environment.SetEnvironmentVariable("ENV_INFO", "test-env-info");

            // Act
            var log = ApplicationLog.GetForExecutingApp(loggerMock.Object);

            // Assert

            Assert.Equal("test-env-info", log.EnvInfo);
            Assert.Equal(PlatformServices.Default.Application.ApplicationName, log.AppName);
            Assert.Equal(PlatformServices.Default.Application.ApplicationVersion, log.AppVersion);
            Assert.NotNull(log.AppName);
            Assert.NotNull(log.AppVersion);
        }
    }
}
