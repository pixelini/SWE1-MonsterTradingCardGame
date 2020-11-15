using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SWE1_REST_HTTP_Webservices;

namespace UnitTests
{
    [TestFixture]
    public class ResponseTest
    {
        [Test]
        public void TestResponseConstructorSpecifiesHttpVersion()
        {
            // Arrange
            string correctHttpVersion = "HTTP/1.1";

            // Act
            var response = new Response
            {
                Status = 201,
                StatusMessage = "Created",
                Body = "ID: 6"
            };

            // Assert
            Assert.AreEqual(correctHttpVersion, response.HttpVersion);
        }


        [Test]
        public void TestResponseWithoutBodyToStringIsCorrect()
        {
            // Arrange
            var response = new Response
            {
                Status = 200,
                StatusMessage = "OK",
            };
            response.Headers.Add("Content-Length", "0");

            string correctResponseString = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 0\r\n\r\n";

            // Act
            string responseString = response.ToString();

            // Assert
            Assert.AreEqual(correctResponseString, responseString);
        }


        [Test]
        public void TestResponseWithBodyToStringIsCorrect()
        {
            // Arrange
            var response = new Response
            {
                Status = 201,
                StatusMessage = "Created",
                Body = "ID: 6"
            };
            response.Headers.Add("Content-Length", response.Body.Length.ToString());

            string correctResponseString = "HTTP/1.1 201 Created\r\nContent-Type: text/plain\r\nContent-Length: 5\r\n\r\nID: 6";

            // Act
            string responseString = response.ToString();

            // Assert
            Assert.AreEqual(correctResponseString, responseString);
        }

    }
}
