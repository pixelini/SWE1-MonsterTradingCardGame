using System.Collections.Generic;
using NUnit.Framework;
using SWE1_REST_HTTP_Webservices;


namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMessageUpdateChangesMsgContent()
        {
            // Arrange
            Message myMessage = new Message(1, "Hello");

            // Act
            var newContent = "Message changed.";
            myMessage.Update(newContent);

            // Assert
            Assert.AreEqual(newContent, myMessage.Content); 
        }

        
        [Test]
        public void TestRequestContextConstructorDefinesCorrectHttpVerb()
        {
            // Arrange
            HttpVerb correctHttpVerb = HttpVerb.Get;

            // Act
            var request = new RequestContext("GET", "/messages", "HTTP/1.1", new Dictionary<string, string>(), "");

            // Assert
            Assert.AreEqual(correctHttpVerb, request.Method);
        }


    }
}



