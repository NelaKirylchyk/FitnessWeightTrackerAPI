using System.Configuration;
using System.Text;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // This can be changed based on endpoint requirements
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use Cookie scheme for sign-in
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        googleOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                // Additional claims or token processing can be done here
                var accessToken = context.AccessToken;
                var userPrincipal = context.Principal;

                // Save or process the access token as needed
            },
        };
    })
    .AddJwtBearer(jwtOptions =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<IBodyWeightService, BodyWeightService>();
builder.Services.AddTransient<IFoodItemService, FoodItemService>();
builder.Services.AddTransient<IFoodRecordService, FoodRecordService>();
builder.Services.AddTransient<INutritionTargetService, NutritionTargetService>();
builder.Services.AddTransient<IUserService, UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));

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

app.UseCors(myAllowSpecificOrigins);

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}