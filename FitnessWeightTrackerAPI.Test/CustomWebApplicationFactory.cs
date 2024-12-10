using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace FitnessWeightTrackerAPI.Test
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public FoodItem FakeFoodItem { get; private set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's FitnessWeightTrackerDbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FitnessWeightTrackerDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0";

                services.AddDbContext<FitnessWeightTrackerDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                // Add IHttpContextAccessor for mocking HttpContext
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<FitnessWeightTrackerDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<FitnessUser>>();
                    var signInManager = scopedServices.GetRequiredService<SignInManager<FitnessUser>>();

                    context.Database.EnsureCreated();

                    // Seed the database with test data
                    var user = new FitnessUser { UserName = "testuser@test.com", Email = "testuser@test.com" };
                    userManager.CreateAsync(user, "Test@123").GetAwaiter().GetResult();

                    var foodItem = new FoodItem
                    {
                        Name = "Banana",
                        Calories = 100,
                        Carbohydrates = 25,
                        Protein = 1,
                        Fat = 5,
                        ServingSize = 100
                    };

                    context.FoodItems.Add(foodItem);
                    context.SaveChanges();

                    FakeFoodItem = foodItem;
                }
            });
        }
    }
}
