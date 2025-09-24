using Microsoft.JSInterop;

namespace FitnessWeightTracker.Client.Services.TokenStore
{
    public sealed class JsTokenStore : ITokenStore
    {
        private readonly IJSRuntime _js;
        private const string TokenStorageKey = "authToken";

        public JsTokenStore(IJSRuntime js) => _js = js;

        public async Task<string?> GetTokenAsync()
        {
            var token = await _js.InvokeAsync<string?>("sessionStorage.getItem", TokenStorageKey);
            if (string.IsNullOrWhiteSpace(token))
                token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);

            return string.IsNullOrWhiteSpace(token) ? null : token;
        }

        public async Task SaveTokenAsync(string token, bool persistent)
        {
            if (persistent)
                await _js.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
            else
                await _js.InvokeVoidAsync("sessionStorage.setItem", TokenStorageKey, token);
        }

        public async Task ClearTokenAsync()
        {
            await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenStorageKey);
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
        }
    }
}