using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Newtonsoft.Json;
using Group1.BackEnd.Domain.Entities;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Group1.BackEnd.Tests.ApiTests.Controllers
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            var content = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/account/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginUser_ReturnsToken()
        {
            // Arrange
            var login = new { Email = "test@example.com", Password = "Password123!" };
            var content = new StringContent(JsonConvert.SerializeObject(login), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/account/login", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("token", responseString);
        }
    }
}
