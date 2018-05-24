using System;
using System.Threading.Tasks;

namespace Common
{
    public interface IMessageProducer<in T>
    {
        Task ProduceAsync(T message);
    }

    public interface IMessageConsumer<out T>
    {
        void Subscribe(Func<T, Task> callback);
    }
}
