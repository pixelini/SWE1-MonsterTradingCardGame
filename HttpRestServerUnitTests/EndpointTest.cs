using NUnit.Framework;
using HttpRestServer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Mtcg;
using Action = HttpRestServer.Action;

namespace UnitTests
{
    [TestFixture]
    public class EndpointTest
    {
        
        
        private EndpointHandler _myEndpointHandler;

        [SetUp]
        public void SetupHandlerAndMessages()
        {
            List<Message> messages  = new List<Message>();
            Message msg1 = new Message(1, "Heyho! What's up?");
            Message msg2 = new Message(2, "Huhu! Just testing around.");
            Message msg3 = new Message(3, "Wow!");
            messages.Add(msg1);
            messages.Add(msg2);
            messages.Add(msg3);
            ConcurrentBag<Battle> allBattles = new ConcurrentBag<Battle>();

        _myEndpointHandler = new EndpointHandler(ref messages, ref allBattles);
        }

        [Test]
        public void Test_HandleRequest_ActionUndefined_StatusBadRequest()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Undefined,
            };

            var correctResponse = new Response
            {
                Status = 400,
                StatusMessage = "Bad Request",
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
        }
        

    }
}
