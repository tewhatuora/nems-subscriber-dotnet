using GuaranteedSubscriber;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SolaceSystems.Solclient.Messaging;
using SolaceSystems.Solclient.Messaging.SDT;
using System;
using System.Text;

namespace GuaranteedSubscriberTest
{
    [TestClass]
    public class SubscriberTest
    {

        [TestMethod]
        public void TestMockMessage()
        {
            //   var mockSession = new Mock<ISession>();
            var mockDestination = new Mock<IDestination>();
            var mockUserPropertyMap = new Mock<IMapContainer>();
            var mockMessage = new Mock<IMessage>();
            byte[] mockPayloadBytes = Encoding.UTF8.GetBytes("Hello, NEMS!");
            sbyte[] mockPayload = Array.ConvertAll(mockPayloadBytes, b => (sbyte)b);


            mockDestination.Setup(d => d.Name).Returns("foo/bah");
            mockUserPropertyMap.Setup(p => p.GetString("source")).Returns("hip");

            // Set up behavior for the GetValue method of the mock

            mockMessage.Setup(m => m.Destination).Returns(mockDestination.Object);
            mockMessage.Setup(m => m.UserPropertyMap).Returns(mockUserPropertyMap.Object);
            mockMessage.Setup(m => m.BinaryAttachment).Returns(mockPayloadBytes);
            Console.WriteLine("This is a test message.");

            Assert.AreEqual("foo/bah", mockMessage.Object.Destination.Name);
            Assert.AreEqual("hip", mockMessage.Object.UserPropertyMap.GetString("source"));
            Assert.AreEqual("Hello, NEMS!", Encoding.ASCII.GetString(mockMessage.Object.BinaryAttachment));

        }

        [TestMethod]
        public void TestNEMSSimulator()
        {
            //   var mockSession = new Mock<ISession>();
            var mockDestination = new Mock<IDestination>();
            var mockUserPropertyMap = new Mock<IMapContainer>();
            var mockMessage = new Mock<IMessage>();
            byte[] mockPayloadBytes = Encoding.UTF8.GetBytes("Hello, NEMS!");
            sbyte[] mockPayload = Array.ConvertAll(mockPayloadBytes, b => (sbyte)b);


            mockDestination.Setup(d => d.Name).Returns("foo/bah");
            mockUserPropertyMap.Setup(p => p.GetString("source")).Returns("hip");

            // Set up behavior for the GetValue method of the mock

            mockMessage.Setup(m => m.Destination).Returns(mockDestination.Object);
            mockMessage.Setup(m => m.UserPropertyMap).Returns(mockUserPropertyMap.Object);
            mockMessage.Setup(m => m.BinaryAttachment).Returns(mockPayloadBytes);

            Console.WriteLine("This is a test message.");
            new EventLoader().ProcessEvent(mockMessage.Object);
        }
    }
}