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



    }
}
