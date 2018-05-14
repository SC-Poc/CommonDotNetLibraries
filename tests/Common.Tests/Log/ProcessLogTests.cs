using System;
using Lykke.Common.Log;
using Moq;
using Xunit;

namespace Common.Tests.Log
{
    public class ProcessLogTests
    {
        [Fact]
        public void All_parameters_passed_correctly()
        {
            // Arrange

            var loggerMock = new Mock<ILogger>();
            var appLog = ApplicationLog.Get(loggerMock.Object, "Lykke.Common.Tests", "1.2.3", "env-info");
            var componentLog = appLog.GetComponentLog("component");
            var log = componentLog.GetProcessLog("process");

            // Act

            log.Trace("message1", "context1", new DateTime(2018, 05, 10));
            log.Debug("message2", "context2", new DateTime(2018, 05, 11));
            log.Info("message3", "context3", new DateTime(2018, 05, 12));
            log.Warning("message4", "context4", new InvalidOperationException("exception4"), new DateTime(2018, 05, 13));
            log.Error("message5", "context5", new InvalidOperationException("exception5"), new DateTime(2018, 05, 14));
            log.FatalError("message6", "context6", new InvalidOperationException("exception6"), new DateTime(2018, 05, 15));

            // Assert

            Assert.Equal("process", log.Process);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.Trace),
                It.Is<string>(s => s == "message1"),
                It.Is<string>(s => s == "context1"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 10))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.Is<string>(s => s == "message2"),
                It.Is<string>(s => s == "context2"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 11))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.Info),
                It.Is<string>(s => s == "message3"),
                It.Is<string>(s => s == "context3"),
                It.Is<Exception>(e => e == null),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 12))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.Is<string>(s => s == "message4"),
                It.Is<string>(s => s == "context4"),
                It.Is<InvalidOperationException>(e => e.Message == "exception4"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 13))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.Is<string>(s => s == "message5"),
                It.Is<string>(s => s == "context5"),
                It.Is<InvalidOperationException>(e => e.Message == "exception5"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 14))), Times.Once);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process"),
                It.Is<LogLevel>(l => l == LogLevel.FatalError),
                It.Is<string>(s => s == "message6"),
                It.Is<string>(s => s == "context6"),
                It.Is<InvalidOperationException>(e => e.Message == "exception6"),
                It.Is<DateTime>(d => d == new DateTime(2018, 05, 15))), Times.Once);

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
                It.IsAny<DateTime>()), Times.Exactly(6));
        }
    }
}