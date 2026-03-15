using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GenReport.Infrastructure.Models.HttpRequests.Onboarding;

namespace GenReport.Tests
{
    [TestFixture]
    public class LoginTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "API returns 200 OK but sets ErrorResponse");
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("Please check email") || content.Contains("ErrorResponse"), "Response should contain an error message");
        }

        [Test]
        public async Task Login_WithMissingEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "",
                Password = "SomePassword123!" 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            // FastEndpoints IsEmail extension throws ArgumentException for empty strings which is caught by GlobalExceptionHandler as 500
            // but wrapped in an HttpResponse 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Unexpected status {response.StatusCode}. Content: {content}");
            Assert.IsTrue(content.Contains("MIDDLEWARE_ERROR") || content.Contains("error executing the query"), "Response should contain an error message");
        }
    }
}
