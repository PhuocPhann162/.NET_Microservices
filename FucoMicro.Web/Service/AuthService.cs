using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;
using static FucoMicro.Web.Utility.SD;

namespace FucoMicro.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;

        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = SD.AuthAPIBase + "/api/auth/register",
                Data = registrationRequestDto
            }, withBearer: false);
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = SD.AuthAPIBase + "/api/auth/login", 
                Data = loginRequestDto,
            }, withBearer: false);
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST, 
                Url = SD.AuthAPIBase + "/api/auth/assignRole", 
                Data = registrationRequestDto,
            });
        }
        
    }
}
