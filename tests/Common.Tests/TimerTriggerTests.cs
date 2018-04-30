using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Log;
using Xunit;

namespace Common.Tests
{
    public class TimerTriggerTests
    {
        [Fact]
        public async Task TimerTrigger_event_triggered_many_times()
        {
            // Arrange
            var timer = new TimerTrigger("TestTimer", TimeSpan.FromMilliseconds(100), new LogToConsole());
            var triggeredCount = 0;

            timer.Triggered += (trigger, args, token) =>
            {
                ++triggeredCount;
                return Task.CompletedTask;
            };

            // Act
            timer.Start();

            await Task.Delay(500);

            timer.Stop();

            // Assert
            Assert.True(triggeredCount > 1);
        }

        [Fact]
        public async Task TimerTrigger_handler_triggered_many_times()
        {
            // Arrange
            var triggeredCount = 0;
            var timer = new TimerTrigger(
                "TestTimer", 
                TimeSpan.FromMilliseconds(100), 
                new LogToConsole(),
                (trigger, args, token) =>
                {
                    ++triggeredCount;
                    return Task.CompletedTask;
                });
            
            // Act
            timer.Start();

            await Task.Delay(500);

            timer.Stop();

            // Assert
            Assert.True(triggeredCount > 1);
        }

        [Fact]
        public async Task TimerTrigger_can_be_run_without_event_handler()
        {
            // Arrange
            var timer = new TimerTrigger("TestTimer", TimeSpan.FromMilliseconds(100), new LogToConsole());

            // Act
            timer.Start();

            await Task.Delay(500);

            timer.Stop();

            // Assert
        }

        [Fact]
        public async Task TimerTrigger_stop_cancels_handler_execution()
        {
            // Arrange
            var timer = new TimerTrigger("TestTimer", TimeSpan.FromMilliseconds(100), new LogToConsole());
            var wasCancellationRequested = false;
            // Act
            timer.Start();

            timer.Triggered += (trigger, args, token) =>
            {
                var sw = Stopwatch.StartNew();

                while (sw.Elapsed < TimeSpan.FromSeconds(5))
                {
                    if (token.IsCancellationRequested)
                    {
                        wasCancellationRequested = true;
                        break;
                    }
                }

                sw.Stop();

                return Task.CompletedTask;
            };

            await Task.Delay(500);

            timer.Stop();

            // Assert
            Assert.True(wasCancellationRequested);
        }
    }
}
