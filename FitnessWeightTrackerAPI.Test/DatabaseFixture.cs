using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Test
{
    public class DatabaseFixture : IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        public HttpClient Client { get; }
        public FitnessWeightTrackerDbContext Context { get; }

        public DatabaseFixture()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            Client = _factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            Context = scopedServices.GetRequiredService<FitnessWeightTrackerDbContext>();

            Context.Database.OpenConnection();
            Context.Database.EnsureCreated();

            // Seed the database
            SeedData(Context);
        }

        private void SeedData(FitnessWeightTrackerDbContext context)
        {
            var random = new Random();

            for (int i = 1; i <= 10; i++)
            {
                var user = new User
                {
                    Name = $"User{i}",
                    Surname = $"Surname{i}",
                    Gender = random.Next(0, 2)
                };

                context.Users.Add(user);
                context.SaveChanges();

                int weightRecordCount = random.Next(1, 11); // 1 to 10 weight records

                for (int j = 1; j <= weightRecordCount; j++)
                {
                    context.BodyWeightRecords.Add(new BodyWeightRecord
                    {
                        Date = DateTime.Now.AddDays(-j),
                        Weight = random.Next(50, 100), // Random weight between 50 and 100
                        UserId = i
                    });
                }
                context.SaveChanges();
            }

            context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.CloseConnection();
            Client.Dispose();
            _factory.Dispose();
        }
    }
}
