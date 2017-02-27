using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Xunit;
using Common.Tests.Stubs;

namespace Common.Tests
{
    public class ProducerConsumerTests
    {
        [Fact]
        public void ProducerShouldBeStartedAutomatically()
        {
            // Produce messages
            // Stop producer
            // Produce another 3 messages
            // Check that all messages are consumed
            // 
            var log = new LogToMemory();
            var pc = new ProducerConsumerStub("component", log);

            pc.ProduceMessage("message 1");
            pc.ProduceMessage("message 2");
            pc.ProduceMessage("message 3");
            Task.Delay(100).Wait();
            pc.Stop();

            pc.ProduceMessage("message 4");
            pc.ProduceMessage("message 5");
            pc.ProduceMessage("message 6");
            Task.Delay(100).Wait();
            pc.Stop();

            Assert.Equal(6, pc.Consumed.Count);

            pc.Dispose();
        }

        [Fact]
        public void ProducingSupportsMultipleThreads()
        {
            // Start 2 threads which produce messages simultaneously.
            // Check that all messages are consumed.
            //
            var log = new LogToMemory();
            var pc = new ProducerConsumerStub("component", log);

            const int COUNT = 1000000;
            Task task1 = new Task(() =>
            {
                for (int i = 0; i < COUNT; i++)
                {
                    pc.ProduceMessage("message" + i);
                }
            });

            Task task2 = new Task(() =>
            {
                for (int i = 0; i < COUNT; i++)
                {
                    pc.ProduceMessage("message" + i);
                }
            });

            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);

            Task.Delay(1000).Wait(); // Wait for all messages to complete

            pc.Stop();

            Assert.Equal(COUNT * 2, pc.Consumed.Count);

            pc.Dispose();
        }

        [Fact]
        public void StartingStoppingSupportsMultipleThreads()
        {
            // Start 2 threads that start, stop producer and produce messages simultaneously.
            // Check that all messages are consumed.
            //
            var log = new LogToMemory();
            var pc = new ProducerConsumerStub("component", log);

            Action<int> action = (i) =>
            {
                pc.Start();
                pc.Stop();
                pc.ProduceMessage("message" + i);
            };

            const int COUNT = 100000;
            Task task1 = new Task(() =>
            {
                for (int i = 0; i < COUNT; i++)
                {
                    action(i);
                }
            });

            Task task2 = new Task(() =>
            {
                for (int i = 0; i < COUNT; i++)
                {
                    action(i);
                }
            });

            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);

            Task.Delay(1000).Wait(); // Wait for all messages to complete

            pc.Stop();

            Assert.Equal(COUNT * 2, pc.Consumed.Count);

            pc.Dispose();
        }
    }
}
