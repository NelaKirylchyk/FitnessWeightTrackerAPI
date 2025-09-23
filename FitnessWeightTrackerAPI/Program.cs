using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program).Assembly);

// Configure Redis
builder.Services.AddStackExchangeRedisCache(
    options => { options.Configuration = builder.Configuration.GetConnectionString("Redis"); });

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
    options.AddPolicy("AllowClient",
        policy => policy
            .WithOrigins("https://localhost:7015")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
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
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
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

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<FitnessUser>();

app.Run();

public partial class Program
{
}