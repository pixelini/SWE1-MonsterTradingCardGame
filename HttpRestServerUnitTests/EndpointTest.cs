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
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body); // AreSame would also compare references
        }


        [Test]
        public void Test_HandleRequest_AddReturnsMsgID()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Add,
            };

            var correctResponse = new Response
            {
                Status = 201,
                StatusMessage = "Created",
                Body = "ID: 1"
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_ReadReturnsMsgContent()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Read,
                ResourcePath = "/messages/3"
            };

            var correctResponse = new Response
            {
                Status = 200,
                StatusMessage = "OK",
                Body = "Wow!"
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_UpdateReturnsNoBody()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Update,
                ResourcePath = "/messages/3",
                Payload = "Not wow!"
            };

            var correctResponse = new Response
            {
                Status = 200,
                StatusMessage = "OK",
                Body = null
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_DeleteReturnsNoBody()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Delete,
                ResourcePath = "/messages/3",
            };

            var correctResponse = new Response
            {
                Status = 200,
                StatusMessage = "OK",
                Body = null
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_ReadNonExistent_StatusNotFound()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Read,
                ResourcePath = "/messages/80",
            };

            var correctResponse = new Response
            {
                Status = 404,
                StatusMessage = "Not Found",
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_UpdateNonExistent_StatusNotFound()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Update,
                ResourcePath = "/messages/80",
            };

            var correctResponse = new Response
            {
                Status = 404,
                StatusMessage = "Not Found",
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
        }


        [Test]
        public void Test_HandleRequest_DeleteNonExistent_StatusNotFound()
        {
            // Arrange
            var request = new RequestContext
            {
                Action = Action.Delete,
                ResourcePath = "/messages/80",
            };

            var correctResponse = new Response
            {
                Status = 404,
                StatusMessage = "Not Found",
            };

            // Act
            Response response = _myEndpointHandler.HandleRequest(request);

            // Assert
            Assert.AreEqual(correctResponse.Status, response.Status);
            Assert.AreEqual(correctResponse.StatusMessage, response.StatusMessage);
            Assert.AreEqual(correctResponse.Body, response.Body);
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
