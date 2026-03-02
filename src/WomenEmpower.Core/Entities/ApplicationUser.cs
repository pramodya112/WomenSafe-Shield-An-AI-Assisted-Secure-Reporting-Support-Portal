using Microsoft.AspNetCore.Identity;

namespace WomenEmpower.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}