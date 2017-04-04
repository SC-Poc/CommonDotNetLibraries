using System;
using System.Threading.Tasks;
using Common;
using Common.Log;

namespace Lykke.Common
{
    public interface IThreadSwitcher
    {
        void SwitchThread(Func<Task> otherThread);
    }

    public interface IThreadSwitcher<T>
    {
        void SwitchThread(Func<T, Task> otherThread, T data);
    }


    public class ThreadSwitcherSingleThread : ProducerConsumer<Func<Task>>, IThreadSwitcher
    {


        public ThreadSwitcherSingleThread(ILog log) : base(nameof(ThreadSwitcherSingleThread), log)
        {
        }

        public void SwitchThread(Func<Task> otherThread)
        {
            Produce(otherThread);
        }

        protected override async Task Consume(Func<Task> item)
        {
            await item();
        }
    }

    public class ThreadSwitcherSingleThread<T> : ProducerConsumer<Tuple<Func<T, Task>, T>>, IThreadSwitcher<T>
    {

        public ThreadSwitcherSingleThread(ILog log) : base(nameof(ThreadSwitcherSingleThread<T>), log)
        {
        }

        public void SwitchThread(Func<T, Task> otherThread, T data)
        {
            Produce(new Tuple<Func<T, Task>, T>(otherThread, data));
        }

        protected override async Task Consume(Tuple<Func<T, Task>, T> item)
        {
            await item.Item1(item.Item2);
        }
    }

    public class ThreadSwitcherToNewTask : IThreadSwitcher
    {
        private readonly ILog _log;


        public ThreadSwitcherToNewTask(ILog log)
        {
            _log = log;
        }

        public void SwitchThread(Func<Task> otherThread)
        {
            Task.Run(async () =>
            {
                try
                {
                    await otherThread();
                }
                catch (Exception ex)
                {
                    await _log.WriteFatalErrorAsync(nameof(ThreadSwitcherToNewTask), "SwitchThread", "", ex);
                }
            });
        }
    }


    public class ThreadSwitcherToNewTask<T> : IThreadSwitcher<T>
    {
        private readonly ILog _log;


        public ThreadSwitcherToNewTask(ILog log)
        {
            _log = log;
        }

        public void SwitchThread(Func<T,Task> otherThread, T data)
        {
            Task.Run(async () =>
            {
                try
                {
                    await otherThread(data);
                }
                catch (Exception ex)
                {
                    await _log.WriteFatalErrorAsync(nameof(ThreadSwitcherToNewTask<T>), "SwitchThread", "", ex);
                }
            });
        }
    }


}
