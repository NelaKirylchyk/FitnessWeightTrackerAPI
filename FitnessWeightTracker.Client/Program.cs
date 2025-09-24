using FitnessWeightTracker.Client;
using FitnessWeightTracker.Client.Services;
using FitnessWeightTracker.Client.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using FitnessWeightTracker.Client.Services.TokenStore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Default HttpClient for same-origin requests
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Blazor authorization
builder.Services.AddAuthorizationCore();

// Token store + auth state provider
builder.Services.AddScoped<ITokenStore, JsTokenStore>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

// Auth header handler
builder.Services.AddScoped<AuthHeaderHandler>();

// AuthService and feature service registrations remain as-is...
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7231/");
});
builder.Services.AddHttpClient<BodyWeightRecordsService>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7231/");
}).AddHttpMessageHandler<AuthHeaderHandler>();

// ViewModel registration: transient + factory to ensure a fresh instance per component
builder.Services.AddTransient<BodyWeightRecordsViewModel>();
builder.Services.AddTransient<BodyWeightRecordsViewModelFactory>(sp => () => sp.GetRequiredService<BodyWeightRecordsViewModel>());

await builder.Build().RunAsync();