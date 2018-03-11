using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Helps working with a collection of tasks and perform group operations with them.
    /// </summary>
    /// <typeparam name="TResult">A result type</typeparam>
    public sealed class TasksManager<TResult>
    {
        private readonly ConcurrentDictionary<object, TaskCompletionSource<TResult>> _tasks = new ConcurrentDictionary<object, TaskCompletionSource<TResult>>();

        /// <summary>
        /// Creates a new task and add it into the internal collection
        /// </summary>
        /// <param name="key">A key</param>
        /// <param name="cancellationToken">An optional cancellation token. Can be used to set the task into the canceled state</param>
        /// <returns>The created task</returns>
        public Task<TResult> Add([NotNull] object key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var task = new TaskCompletionSource<TResult>(key);
            if (_tasks.TryAdd(key, task))
            {
                cancellationToken.Register(() => Cancel(key, cancellationToken));
            }

            return task.Task;
        }

        /// <summary>
        /// Attempts to transition the  task into the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion"></see> state.
        /// </summary>
        /// <param name="key">The task key</param>
        /// <param name="result">The <paramref name="result"/> to set into the task</param>
        public void SetResult([NotNull] object key, TResult result)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_tasks.TryRemove(key, out var task))
            {
                task.TrySetResult(result);
            }
        }


        /// <summary>
        /// Attempts to transition the task into the <see cref="F:System.Threading.Tasks.TaskStatus.Canceled"></see> state.
        /// </summary>
        /// <param name="key">A key of the task</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        public void Cancel([NotNull] object key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_tasks.TryRemove(key, out var task))
            {
                task.TrySetCanceled(cancellationToken);
            }
        }

        /// <summary>
        /// Attempts to transition all tasks into the <see cref="F:System.Threading.Tasks.TaskStatus.Canceled"></see> state.
        /// </summary>
        /// <param name="cancellationToken">An optional cancellation token</param>
        public void CancelAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var key in _tasks.Keys)
            {
                Cancel(key, cancellationToken);
            }
        }

        /// <summary>
        /// Attempts to transition all tasks into the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted"></see> state.
        /// </summary>
        /// <param name="exception">An <paramref name="exception"/> to set in to task</param>
        public void SetExceptionsToAll([NotNull] Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            foreach (var key in _tasks.Keys)
            {
                if (_tasks.TryRemove(key, out var task))
                {
                    task.TrySetException(exception);
                }
            }
        }
    }
}
