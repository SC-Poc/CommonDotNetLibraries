using System.Threading.Tasks;
using Xunit;
using Common.Tests.Stubs;
using Lykke.Common.Log;
using Moq;

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
            var logFactory = new Mock<ILogFactory>();
            var pc = new ProducerConsumerStub("component", logFactory.Object);

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

        }



    }
}
