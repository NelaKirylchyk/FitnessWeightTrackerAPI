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

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7231/";

// Named HttpClients
var api = builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri(apiBase);
});

var apiAuth = builder.Services.AddHttpClient("ApiAuth", c =>
{
    c.BaseAddress = new Uri(apiBase);
}).AddHttpMessageHandler<AuthHeaderHandler>();

api.AddTypedClient<AuthService>((http, sp) =>
    new AuthService(http, sp.GetRequiredService<ITokenStore>(), sp.GetRequiredService<JwtAuthenticationStateProvider>()));

apiAuth.AddTypedClient<BodyWeightRecordsService>((http, sp) =>
    new BodyWeightRecordsService(http));

apiAuth.AddTypedClient<BodyWeightTargetsService>((http, sp) =>
    new BodyWeightTargetsService(http));

apiAuth.AddTypedClient<FoodItemsService>((http, sp) =>
    new FoodItemsService(http));

apiAuth.AddTypedClient<FoodRecordsService>((http, sp) =>
    new FoodRecordsService(http));

apiAuth.AddTypedClient<NutritionTargetsService>((http, sp) =>
    new NutritionTargetsService(http));


await builder.Build().RunAsync();