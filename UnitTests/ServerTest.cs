using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SWE1_REST_HTTP_Webservices;
using Moq;
using System.Net;
using Action = SWE1_REST_HTTP_Webservices.Action; // because Action is also used in another context

namespace UnitTests
{
    [TestFixture]
    public class ServerTest
    {
        private Mock<IClient> MockedClient { get; set; }
        private Mock<IEndpointHandler> MockedEndpointHandler { get; set; }
        private HttpServer MyServer { get; set; }


        [SetUp]
        public void SetupMocks()
        {
            MockedClient = new Mock<IClient>();
            MockedEndpointHandler = new Mock<IEndpointHandler>();
            MyServer = new HttpServer(IPAddress.Loopback, 6789, "/messages", MockedEndpointHandler.Object);
            MockedClient.Setup(x => x.SendResponse(It.IsAny<Response>())).Callback(() => { });
        }


        [Test]
        public void Test_ProcessRequest_ReturnsListAction()
        {
            // Arrange
            var correctAction = Action.List;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/messages",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }


        [Test]
        public void Test_ProcessRequest_ReturnsAddAction()
        {
            // Arrange
            var correctAction = Action.Add;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/messages",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }


        [Test]
        public void Test_ProcessRequest_ReturnsReadAction()
        {
            // Arrange
            var correctAction = Action.Read;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/messages/1",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }


        [Test]
        public void Test_ProcessRequest_ReturnsUpdateAction()
        {
            // Arrange
            var correctAction = Action.Update;
            var request = new RequestContext
            {
                Method = HttpVerb.Put,
                ResourcePath = "/messages/1",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }


        [Test]
        public void Test_ProcessRequest_ReturnsDeleteAction()
        {
            // Arrange
            var correctAction = Action.Delete;
            var request = new RequestContext
            {
                Method = HttpVerb.Delete,
                ResourcePath = "/messages/1",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsUndefinedAction()
        {
            // Arrange
            var correctAction = Action.Undefined;
            var request = new RequestContext
            {
                Method = HttpVerb.Delete,
                ResourcePath = "/not/valid",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

    }
}
