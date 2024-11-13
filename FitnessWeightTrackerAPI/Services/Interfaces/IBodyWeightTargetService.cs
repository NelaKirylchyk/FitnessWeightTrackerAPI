using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface IBodyWeightTargetService
    {
        public Task UpdateBodyweightTarget(int id, int userId, BodyWeightTargetDTO targetWeight);

        public Task<BodyWeightTarget> GetUserBodyweightTarget(int userId);

        public Task<BodyWeightTarget> AddBodyweightTarget(int userId, BodyWeightTargetDTO target);

        public Task DeleteBodyweightTarget(int id, int userId);
    }
}
