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
        public void StatusIsValid()
        {
            // Arrange
            Status myStatus = new Status(200, "OK");

            // Act
            var code = myStatus.Code;

            // Assert
            
        }
    }
}