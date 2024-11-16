using Microsoft.AspNetCore.Identity;

namespace FitnessWeightTrackerAPI.Models
{
    public class FitnessUser : IdentityUser<int>
    {
    }

    public class FitnessUserRole : IdentityRole<int>
    {
    }
}
