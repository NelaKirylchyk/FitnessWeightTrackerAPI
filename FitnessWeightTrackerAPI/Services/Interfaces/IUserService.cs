using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> RegisterUserAsync(RegistrationUserDTO user);

        Task<User> LoginUserAsync(UserLoginDTO login);
        Task<bool> DeleteUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);

        int? GetUserIdFromAuth(ControllerBase controller);

        string GenerateUserJWTToken(User user);
    }
}
