using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Threadsafe queue with confirmation of success of handling message
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public class QueueWithConfirmation<T>
    {
        public class QueueItem : IDisposable
        {
            private readonly QueueWithConfirmation<T> _queue;
            private bool _disposed;

            internal QueueItem(QueueWithConfirmation<T> queue, T item)
            {
                _queue = queue;
                Item = item;
            }

            public T Item { get; }


            private bool _isComplieted;
            public void Compliete()
            {
                _isComplieted = true;
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
            
                if (!_isComplieted)
                    _queue.Enqueue(Item);
            
                _disposed = true;
            }
        }

        private readonly Queue<T> _queue = new Queue<T>();

        public void Enqueue(T item)
        {
            lock (_queue)
                _queue.Enqueue(item);
        }

        public QueueItem Dequeue()
        {
            lock (_queue)
            {
                if (_queue.Count>0)
                    return new QueueItem(this, _queue.Dequeue());
            }

            return null;
        }
    }
}
