using FucoMicro.Services.AuthAPI.Models.Dto;

namespace FucoMicro.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        
    }
}
