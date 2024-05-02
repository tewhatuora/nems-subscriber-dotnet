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
            mockUserPropertyMap.Setup(p => p.GetString("id")).Returns("1234");

            // Set up behavior for the GetValue method of the mock

            mockMessage.Setup(m => m.Destination).Returns(mockDestination.Object);
            mockMessage.Setup(m => m.UserPropertyMap).Returns(mockUserPropertyMap.Object);
            mockMessage.Setup(m => m.BinaryAttachment).Returns(mockPayloadBytes);
            Console.WriteLine("This is a test message.");

            Assert.AreEqual("foo/bah", mockMessage.Object.Destination.Name);
            Assert.AreEqual("hip", mockMessage.Object.UserPropertyMap.GetString("source"));
            Assert.AreEqual("1234", mockMessage.Object.UserPropertyMap.GetString("id"));
            Assert.AreEqual("Hello, NEMS!", Encoding.ASCII.GetString(mockMessage.Object.BinaryAttachment));

        }

        [TestMethod]
        public void TestNEMSSimulator()
        {
            //   var mockSession = new Mock<ISession>();
            var mockDestination = new Mock<IDestination>();
            var mockUserPropertyMap = new Mock<IMapContainer>();
            var mockMessage = new Mock<IMessage>();
            byte[] mockPayloadBytes = Encoding.UTF8.GetBytes("{\"callbackUrl\": \"https://api.hip-uat.digital.health.nz/fhir/nhi/v1/Patient/ZAT2348\", \"deathDate\": \"2016-02-18\"}");
            sbyte[] mockPayload = Array.ConvertAll(mockPayloadBytes, b => (sbyte)b);


            mockDestination.Setup(d => d.Name).Returns("demographics/patient/death/new/0.1.0/G00036-D/2203/FZZ988-H/ZAT2348");
            mockUserPropertyMap.Setup(p => p.GetString("source")).Returns("https://hip.uat.digital.health.nz");
            mockUserPropertyMap.Setup(p => p.GetString("id")).Returns("86152393-fc5b-4b21-a315-5992f31d2aa1");
            mockUserPropertyMap.Setup(p => p.GetString("datacontenttype")).Returns("application/json");
            mockUserPropertyMap.Setup(p => p.GetString("time")).Returns("2024-04-08T04:18:47.11Z");
            mockUserPropertyMap.Setup(p => p.GetString("type")).Returns("demographics/patient/death/new/0.1.0");
            mockUserPropertyMap.Setup(p => p.GetString("subject")).Returns("ZAT2348");

            // Set up behavior for the GetValue method of the mock

            mockMessage.Setup(m => m.Destination).Returns(mockDestination.Object);
            mockMessage.Setup(m => m.UserPropertyMap).Returns(mockUserPropertyMap.Object);
            mockMessage.Setup(m => m.BinaryAttachment).Returns(mockPayload);
            mockMessage.Setup(m => m.HttpContentType).Returns("application/json");
            // mockMessage.Setup(m => m.HttpContentEncoding).Returns("br");

            new EventLoader().ProcessEvent(mockMessage.Object);

        }
    }
}