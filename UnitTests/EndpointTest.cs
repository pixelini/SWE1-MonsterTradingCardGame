using NUnit.Framework;
using SWE1_REST_HTTP_Webservices;
using System;
using System.Collections.Generic;
using System.Text;
using Action = SWE1_REST_HTTP_Webservices.Action;

namespace UnitTests
{
    [TestFixture]
    public class EndpointTest
    {
        private EndpointHandler MyEndpointHandler;

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
            MyEndpointHandler = new EndpointHandler(ref messages);

        }

        [Test]
        public void Test_HandleRequest_ListReturnsMessages()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.List,
            };

            var correctResponse = new Response
            {
                Status = 200,
                StatusMessage = "OK",
                Body = "ID 1:\nHeyho! What's up?\n\nID 2:\nHuhu! Just testing around.\n\nID 3:\nWow!\n\n"
            };
            
            // Act
            Response response = MyEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Body, response.Body); // AreSame would also compare references
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
        }



    }
}
