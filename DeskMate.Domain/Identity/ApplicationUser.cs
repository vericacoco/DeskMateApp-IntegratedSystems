using Microsoft.AspNetCore.Identity;

namespace DeskMateApp.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? FullName { get; set; }
        public string? Department { get; set; }

        public string? City { get; set; }
    }
}
