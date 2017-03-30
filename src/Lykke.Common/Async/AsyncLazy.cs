using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Common.Async
{
    /// <summary>
    /// Class responsible for lazy async initialization
    /// ALWAYS await on Value please(Or at least use it like Value.Result)
    /// Source information: https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory, LazyThreadSafetyMode mode) :
            base(() => Task.Factory.StartNew(valueFactory), mode)
        { }
        public AsyncLazy(Func<Task<T>> taskFactory, LazyThreadSafetyMode mode) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
}
