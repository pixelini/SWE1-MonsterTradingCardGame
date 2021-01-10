using System.Collections.Generic;
using NUnit.Framework;
using HttpRestServer;


namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        
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



