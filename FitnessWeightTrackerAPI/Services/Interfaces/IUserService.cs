namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    using FitnessWeightTrackerAPI.Data.DTO;
    using FitnessWeightTrackerAPI.Models;

    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);

        Task<User> RegisterUserAsync(RegistrationUserDTO user, bool isExternalUser = false);

        Task<User> LoginUserAsync(UserLoginDTO login);

        Task<bool> DeleteUserAsync(int id);

        Task<User> GetUserByEmailAsync(string email);

        string GenerateUserJWTToken(User user);
    }
}
