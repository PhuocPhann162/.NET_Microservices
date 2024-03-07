using Microsoft.AspNetCore.Identity;

namespace FucoMicro.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
