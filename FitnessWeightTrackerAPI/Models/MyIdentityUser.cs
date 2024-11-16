using Microsoft.AspNetCore.Identity;

namespace FitnessWeightTrackerAPI.Models
{
    public class MyIdentityUser : IdentityUser<int>
    {
    }

    public class MyIdentityUserRole : IdentityRole<int>
    { // Additional properties if needed }
    }
}
