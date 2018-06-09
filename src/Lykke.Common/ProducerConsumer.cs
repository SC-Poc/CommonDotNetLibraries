using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Common
{
    /// <summary>
    /// Producer-consumer pattern implementation.
    /// </summary>
    /// <typeparam name="T">Item type to produce and consume</typeparam>
    [PublicAPI]
    public abstract class ProducerConsumer<T> : IStartable, IStopable where T : class
    {
        private readonly object _startStopLockobject = new object();
        private readonly Queue<TaskCompletionSource<T>> _queue = new Queue<TaskCompletionSource<T>>();
        private readonly string _metricName;
        private readonly bool _isAppInisghtsMetricEnabled;
        private CancellationTokenSource _cancellation;
        private bool _disposed;

        [Obsolete("Use ComponentName")]
        protected readonly string _componentName;

        public string ComponentName => _componentName;

        private bool _started;
        private Task _threadTask;
        private TaskCompletionSource<T> _last;
        private readonly ILog _log;

        [Obsolete("Use your own log")]
        protected ILog Log => _log;

        /// <summary>
        /// Override this method to consume next item
        /// </summary>
        protected virtual Task Consume(T item)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method to consume next item with possibility to interrupt execution using cancellationToken
        /// </summary>
        protected virtual Task Consume(T item, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            return Consume(item);
        }

        [Obsolete("Use protected ProducerConsumer([NotNull] string componentName, [NotNull] ILogFactory logFactory, bool enableAppInisghtsMetric = false)")]
        protected ProducerConsumer(string componentName, ILog log)
            : this(componentName, log, false)
        {
        }

        [Obsolete("Use protected ProducerConsumer([NotNull] string componentName, [NotNull] ILogFactory logFactory, bool enableAppInisghtsMetric = false)")]
        protected ProducerConsumer(ILog log)
            : this(null, log, false)
        {
        }

        [Obsolete("Use protected ProducerConsumer([NotNull] string componentName, [NotNull] ILogFactory logFactory, bool enableAppInisghtsMetric = false)")]
        protected ProducerConsumer(
            string componentName,
            ILog log,
            bool enableAppInisghtsMetric)
        {
            if (string.IsNullOrWhiteSpace(componentName))
            {
                _metricName = $"ProducerConsumer<{typeof(T).Name}> count";
            }
            else
            {
                _componentName = componentName;
                _metricName = $"ProducerConsumer<{typeof(T).Name}> count for {_componentName}";    
            }
            _isAppInisghtsMetricEnabled = enableAppInisghtsMetric;
            
            _log = log;
        }

        protected ProducerConsumer(
            [NotNull] ILogFactory logFactory,
            [CanBeNull] string componentName = null,
            bool enableAppInisghtsMetric = false)
        {
            if (string.IsNullOrWhiteSpace(componentName))
            {
                _metricName = $"ProducerConsumer<{typeof(T).Name}> count";
            }
            else
            {
                _componentName = componentName;
                _metricName = $"ProducerConsumer<{typeof(T).Name}> count for {_componentName}";
            }
            _isAppInisghtsMetricEnabled = enableAppInisghtsMetric;

            _log = componentName == null ? logFactory.CreateLog(this) : logFactory.CreateLog(this, componentName);
        }

        private TaskCompletionSource<T> Dequeue()
        {
            lock (_queue)
                return _queue.Dequeue();
        }

        private async Task StartTask()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        var task = Dequeue();
                        var value = await task.Task;

                        if (value == null)
                            return;

                        await Consume(value, _cancellation.Token).ConfigureAwait(false);
                        
                        if (_isAppInisghtsMetricEnabled)
                            ApplicationInsightsTelemetry.TrackMetric(_metricName, _queue.Count);
                    }
                    catch (Exception exception)
                    {
                        await LogErrorAsync(exception);
                    }
                }
                // Saves the loop if nothing didn't help
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }

        private async Task LogErrorAsync(Exception exception)
        {
            try
            {
                if (Log != null)
                {
                    if (string.IsNullOrWhiteSpace(_componentName))
                        await Log.WriteErrorAsync("Handle", "", exception);
                    else
                        await _log.WriteErrorAsync(ComponentName, "Handle", "", exception);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        /// Produces next item. If producer-consumer is not started yet, then it will be started automatically
        /// </summary>
        protected void Produce(T item)
        {
            Start();

            lock (_queue)
            {
                var last = _last;
                _last = new TaskCompletionSource<T>();
                _queue.Enqueue(_last);
                if (_isAppInisghtsMetricEnabled)
                    ApplicationInsightsTelemetry.TrackMetric(_metricName, _queue.Count);
                last.SetResult(item);
            }

        }

        /// <summary>
        /// Starts producer-consumer
        /// </summary>
        public virtual void Start()
        {
            if (_started)
                return;

            lock (_startStopLockobject)
            {
                if (_started)
                    return;

                _started = true;
            }

            _cancellation = new CancellationTokenSource();
            _last = new TaskCompletionSource<T>();
            lock (_queue)
                _queue.Enqueue(_last);

            _threadTask = StartTask();
        }

        /// <summary>
        /// Stops producer-consumer. Synchronously waits until produced items queue became empty
        /// </summary>
        public virtual void Stop()
        {
            if (!_started)
                return;

            lock (_startStopLockobject)
            {
                if (!_started)
                    return;
                _started = false;
            }

            _cancellation?.Cancel();

            _last.SetResult(null);
            _threadTask.ConfigureAwait(false).GetAwaiter().GetResult();

            _cancellation?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return; 
            
            Stop();
            
            _disposed = true;
        }
    }
}
