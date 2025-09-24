using FitnessWeightTracker.Client.Services.TokenStore;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitnessWeightTracker.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ITokenStore _tokenStore;
        private readonly JwtAuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient http, ITokenStore tokenStore, JwtAuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _tokenStore = tokenStore;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> RegisterAsync(FitnessWeightTracker.Client.Models.RegisterModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", model);
            return response.IsSuccessStatusCode;
        }

        // Stores only the JWT value from the JSON: {"token":"<jwt>"}
        public async Task<string?> LoginAsync(FitnessWeightTracker.Client.Models.LoginModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", model);
            if (!response.IsSuccessStatusCode)
                return null;

            var obj = await response.Content.ReadFromJsonAsync<LoginResponse>();
            var token = obj?.Token;

            if (string.IsNullOrWhiteSpace(token))
            {
                var raw = await response.Content.ReadAsStringAsync();
                token = ExtractTokenFromJson(raw);
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                await _tokenStore.SaveTokenAsync(token, persistent: false);
                _authStateProvider.NotifyUserAuthentication(token);
                return token;
            }

            return null;
        }

        public async Task<bool> TryRestoreTokenAsync()
        {
            var token = await _tokenStore.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                _authStateProvider.NotifyUserAuthentication(token);
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _tokenStore.ClearTokenAsync();
            _authStateProvider.NotifyUserLogout();
        }

        private static string? ExtractTokenFromJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("token", out var t) && t.ValueKind == JsonValueKind.String)
                    return t.GetString();
            }
            catch
            {
            }
            return null;
        }

        private sealed class LoginResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; } = string.Empty;
        }
    }
}