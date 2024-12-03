using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//// Configure logging provider
//builder.Logging.ClearProviders(); // Optional: Clears default loggingproviders
//builder.Logging.AddConsole(); // Adds console logging provider
//builder.Logging.AddDebug(); // Adds Debug window logging provider

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7231")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddDbContext<FitnessWeightTrackerDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.BearerScheme;
})
    .AddCookie(IdentityConstants.ExternalScheme)
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        googleOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var accessToken = context.AccessToken;
                var userPrincipal = context.Principal;
            }
        };
        googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
    });

builder.Services.AddIdentityCore<FitnessUser>()
    .AddRoles<FitnessUserRole>()
    .AddEntityFrameworkStores<FitnessWeightTrackerDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<FitnessUser>>()
    .AddApiEndpoints();

builder.Services.AddAuthorization();

builder.Services.AddTransient<IBodyWeightService, BodyWeightService>();
builder.Services.AddTransient<IBodyWeightTargetService, BodyWeightTargetService>();
builder.Services.AddTransient<IFoodItemService, FoodItemService>();
builder.Services.AddTransient<IFoodRecordService, FoodRecordService>();
builder.Services.AddTransient<INutritionTargetService, NutritionTargetService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header required",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            new string[] { }
        },
    });
});

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<FitnessUser>();

app.Run();

public partial class Program
{
}