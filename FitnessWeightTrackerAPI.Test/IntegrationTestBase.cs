using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace FitnessWeightTrackerAPI.Test
{
    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory<Program> Factory;
        private static string _authToken;

        protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            AuthenticateClientOnce().GetAwaiter().GetResult();
        }

        private async Task AuthenticateClientOnce()
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                return;
            }

            var loginRequest = new LoginRequest
            {
                Email = "testuser@test.com",
                Password = "Test@123"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _authToken = loginResponse.AccessToken;

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
