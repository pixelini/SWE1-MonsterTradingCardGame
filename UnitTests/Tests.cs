using NUnit.Framework;
using SWE1_REST_HTTP_Webservices;


namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MessageUpdate()
        {
            // Arrange
            Message myMessage = new Message(1, "Hello");

            // Act
            var newContent = "Message changed.";
            myMessage.Update(newContent);

            // Assert
            Assert.AreEqual(myMessage.Content, newContent);
        }


    }
}