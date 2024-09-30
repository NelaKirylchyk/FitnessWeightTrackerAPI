using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace FitnessWeightTrackerAPI.Data
{
    public class FitnessWeightTrackerDbContext : DbContext
    {
        public FitnessWeightTrackerDbContext(DbContextOptions<FitnessWeightTrackerDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<BodyWeightRecord> BodyWeightRecords { get; set; }
        public DbSet<BodyWeightTarget> BodyWeightTargets { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodRecord> FoodRecords { get; set; }
        public DbSet<NutritionTarget> NutritionTargets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasData(new User {
                    Id = 1,
                    Name = "New temp user name",
                    BirthDate = DateTime.UtcNow,
                    Gender = "W", 
                    Surname = "New temp user surname"});
        }

    }
}
