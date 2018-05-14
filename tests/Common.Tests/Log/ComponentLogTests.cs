using System;
using Lykke.Common.Log;
using Moq;
using Xunit;

namespace Common.Tests.Log
{
    public class ComponentLogTests
    {
        [Fact]
        public void All_parameters_passed_correctly()
        {
            // Arrange

            var loggerMock = new Mock<ILogger>();
            var appLog = ApplicationLog.Get(loggerMock.Object, "Lykke.Common.Tests", "1.2.3", "env-info");
            var log = appLog.GetComponentLog("component");

            // Act

            var processLog0 = log.GetProcessLog("process0");
            var processLog1 = log.GetEmptyProcessLog();

            log.Trace("process1", "message1", "context1", new DateTime(2018, 05, 10));
            log.Debug("process2", "message2", "context2", new DateTime(2018, 05, 11));
            log.Info("process3", "message3", "context3", new DateTime(2018, 05, 12));
            log.Warning("process4", "message4", "context4", new InvalidOperationException("exception4"), new DateTime(2018, 05, 13));
            log.Error("process5", "message5", "context5", new InvalidOperationException("exception5"), new DateTime(2018, 05, 14));
            log.FatalError("process6", "message6", "context6", new InvalidOperationException("exception6"), new DateTime(2018, 05, 15));

            // Assert
            
            Assert.Equal("component", log.Component);

            Assert.Equal("process0", processLog0.Process);
            Assert.Null(processLog1.Process);

            loggerMock.Verify(m => m.Write(
                It.Is<string>(s => s == "Lykke.Common.Tests"),
                It.Is<string>(s => s == "1.2.3"),
                It.Is<string>(s => s == "env-info"),
                It.Is<string>(s => s == "component"),
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
                It.Is<string>(s => s == "component"),
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
                It.Is<string>(s => s == "component"),
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
                It.Is<string>(s => s == "component"),
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
                It.Is<string>(s => s == "component"),
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
                It.Is<string>(s => s == "component"),
                It.Is<string>(s => s == "process6"),
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