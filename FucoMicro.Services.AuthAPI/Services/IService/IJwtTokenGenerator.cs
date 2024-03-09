using FucoMicro.Services.AuthAPI.Models;

namespace FucoMicro.Services.AuthAPI.Services.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
