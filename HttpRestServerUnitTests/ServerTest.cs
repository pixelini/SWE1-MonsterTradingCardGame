using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using HttpRestServer;
using Moq;
using System.Net;
using Action = HttpRestServer.Action; // because Action is also used in another context

namespace UnitTests
{
    [TestFixture]
    public class ServerTest
    {
        private Mock<IClient> MockedClient { get; set; }
        private Mock<IEndpointHandler> MockedEndpointHandler { get; set; }
        private Mock<IDatabase> MockedDatabase { get; set; }
        private HttpServer MyServer { get; set; }


        [SetUp]
        public void SetupMocks()
        {
            MockedClient = new Mock<IClient>();
            MockedEndpointHandler = new Mock<IEndpointHandler>();
            MockedDatabase = new Mock<IDatabase>();
            MyServer = new HttpServer(IPAddress.Loopback, 6789, "/messages", MockedEndpointHandler.Object);
            MockedClient.Setup(x => x.SendResponse(It.IsAny<Response>())).Callback(() => { });
        }

        [Test]
        public void Test_ProcessRequest_ReturnsRegistrationAction()
        {
            // Arrange
            var correctAction = Action.Registration;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/users",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsLoginAction()
        {
            // Arrange
            var correctAction = Action.Login;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/sessions",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsAddPackageAction()
        {
            // Arrange
            var correctAction = Action.AddPackage;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/packages",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsBuyPackageAction()
        {
            // Arrange
            var correctAction = Action.BuyPackage;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/transactions/packages",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowCardsAction()
        {
            // Arrange
            var correctAction = Action.ShowCards;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/cards",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowDeckAction()
        {
            // Arrange
            var correctAction = Action.ShowDeck;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/deck",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsConfigureDeckAction()
        {
            // Arrange
            var correctAction = Action.ConfigureDeck;
            var request = new RequestContext
            {
                Method = HttpVerb.Put,
                ResourcePath = "/deck",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowDeckInPlainTextAction()
        {
            // Arrange
            var correctAction = Action.ShowDeckInPlainText;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/deck?format=plain",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowStatsAction()
        {
            // Arrange
            var correctAction = Action.ShowStats;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/stats",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowScoreboardAction()
        {
            // Arrange
            var correctAction = Action.ShowScoreboard;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/score",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsJoinBattleAction()
        {
            // Arrange
            var correctAction = Action.JoinBattle;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/battles",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsShowDealsAction()
        {
            // Arrange
            var correctAction = Action.ShowDeals;
            var request = new RequestContext
            {
                Method = HttpVerb.Get,
                ResourcePath = "/tradings",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

        [Test]
        public void Test_ProcessRequest_ReturnsCreateDealAction()
        {
            // Arrange
            var correctAction = Action.CreateDeal;
            var request = new RequestContext
            {
                Method = HttpVerb.Post,
                ResourcePath = "/tradings",
            };

            MockedClient.Setup(client => client.ReceiveRequest()).Returns(() => request);

            // Act
            MyServer.ProcessRequest(MockedClient.Object);

            // Assert
            MockedEndpointHandler.Verify(x => x.HandleRequest(It.Is<RequestContext>(y => y.Action.Equals(correctAction))));
        }

    }
}
