using Microsoft.AspNetCore.Identity;

namespace MvcWebApp.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; } // Custom property for User's Name
    }
}