using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Data
{
    public class FitnessWeightTrackerDbContext : IdentityDbContext<FitnessUser, FitnessUserRole, int>
    {
        public FitnessWeightTrackerDbContext(DbContextOptions<FitnessWeightTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<BodyWeightRecord> BodyWeightRecords { get; set; }

        public DbSet<BodyWeightTarget> BodyWeightTargets { get; set; }

        public DbSet<FoodItem> FoodItems { get; set; }

        public DbSet<FoodRecord> FoodRecords { get; set; }

        public DbSet<NutritionTarget> NutritionTargets { get; set; }

    }
}
