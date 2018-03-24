using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Common.Tests.UtilsClass
{
    public class TasksManagerTest
    {
        private readonly TasksManager<int> _tasksManager;
        public TasksManagerTest()
        {
            _tasksManager = new TasksManager<int>();
        }

        [Fact]
        public void ShouldCreateNewTask()
        {
            var task = _tasksManager.Add(42);

            Assert.NotNull(task);
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
        }

        [Fact]
        public void ShouldCompleteTask()
        {
            var task = _tasksManager.Add(42);
            _tasksManager.SetResult(42, 43);

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        }

        [Fact]
        public void ShouldCancelTask()
        {
            var task = _tasksManager.Add(42);
            _tasksManager.Cancel(42);
            Assert.Equal(TaskStatus.Canceled, task.Status);
        }

        [Fact]
        public void ShouldCancelAll()
        {
            var task1 = _tasksManager.Add(42);
            var task2 = _tasksManager.Add(43);
            _tasksManager.CancelAll();

            Assert.Equal(TaskStatus.Canceled, task1.Status);
            Assert.Equal(TaskStatus.Canceled, task2.Status);
        }

        [Fact]
        public void ShouldCancelByTimeout()
        {
            var now = DateTime.Now;
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0.5)))
            {
                var task = _tasksManager.Add(42, cts.Token);
                var cancelled = Task.WaitAny(task, Task.Delay(TimeSpan.FromSeconds(1)));
                var elapsed = DateTime.Now - now;

                Assert.Equal(0, cancelled);
                Assert.Equal(TaskStatus.Canceled, task.Status);
                Assert.True(elapsed.TotalSeconds < 1, $"Actual elapsed time = {elapsed.TotalSeconds} (>= 1 sec)");
                Assert.True(elapsed.TotalSeconds >= 0.5, $"Actual elapsed time = {elapsed.TotalSeconds} (< 0.5 sec)");
            }
        }

        [Fact]
        public void ShouldSetException()
        {
            var task = _tasksManager.Add(42);
            var exc = new InvalidOperationException();
            _tasksManager.SetExceptionsToAll(exc);
            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.Equal(exc, task.Exception.InnerExceptions[0]);
        }
    }
}
