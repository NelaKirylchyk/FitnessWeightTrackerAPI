using System.Threading.Tasks;

namespace FitnessWeightTracker.Client.Services.TokenStore
{
    public interface ITokenStore
    {
        Task<string?> GetTokenAsync();
        Task SaveTokenAsync(string token, bool persistent);
        Task ClearTokenAsync();
    }
}