using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SWE1_REST_HTTP_Webservices;
using Moq;
using System.Net;

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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.LIST;
            var request = new RequestContext
            {
                Method = HttpVerb.GET,
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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.ADD;
            var request = new RequestContext
            {
                Method = HttpVerb.POST,
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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.READ;
            var request = new RequestContext
            {
                Method = HttpVerb.GET,
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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.UPDATE;
            var request = new RequestContext
            {
                Method = HttpVerb.PUT,
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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.DELETE;
            var request = new RequestContext
            {
                Method = HttpVerb.DELETE,
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
            var correctAction = SWE1_REST_HTTP_Webservices.Action.UNDEFINED;
            var request = new RequestContext
            {
                Method = HttpVerb.DELETE,
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
