using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface INutritionTargetService
    {
        public Task UpdateNutritionTarget(int id, int userId, NutritionTargetDTO targetWeight);

        public Task<NutritionTarget> GetNutritionTarget(int userId);

        public Task<NutritionTarget> AddNutritionTarget(int userId, NutritionTargetDTO target);

        public Task DeleteNutritionTarget(int id, int userId);
    }
}
