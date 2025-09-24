using System.Security.Claims;
using System.Text.Json;
using FitnessWeightTracker.Client.Services.TokenStore;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitnessWeightTracker.Client.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStore _tokenStore;
        private static readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public JwtAuthenticationStateProvider(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStore.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var user = CreateClaimsPrincipalFromToken(token);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var user = CreateClaimsPrincipalFromToken(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private static ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), authenticationType: "jwt");
            if (identity.FindFirst(identity.NameClaimType) is null)
            {
                var email = identity.FindFirst(ClaimTypes.Email)?.Value
                            ?? identity.FindFirst("email")?.Value;
                if (!string.IsNullOrWhiteSpace(email))
                    identity.AddClaim(new Claim(ClaimTypes.Name, email));
            }
            return new ClaimsPrincipal(identity);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) return Array.Empty<Claim>();

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var kvp = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new();

            var claims = new List<Claim>();
            foreach (var pair in kvp)
            {
                if (pair.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                            claims.Add(new Claim(pair.Key, item.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim(pair.Key, element.ToString()));
                    }
                }
                else if (pair.Value != null)
                {
                    claims.Add(new Claim(pair.Key, pair.Value.ToString()!));
                }
            }

            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (!string.IsNullOrWhiteSpace(email) && claims.All(c => c.Type != ClaimTypes.Email))
                claims.Add(new Claim(ClaimTypes.Email, email));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}