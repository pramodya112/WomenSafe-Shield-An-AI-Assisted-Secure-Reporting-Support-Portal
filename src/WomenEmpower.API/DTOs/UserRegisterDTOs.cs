
namespace WomenEmpower.API.DTOs
{
    public class UserRegisterDTOs
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? OrganizationName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}