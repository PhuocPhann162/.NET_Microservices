using FucoMicro.Services.AuthAPI.Data;
using FucoMicro.Services.AuthAPI.Models.Dto;
using FucoMicro.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FucoMicro.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _response = new ResponseDto();
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessages = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessages))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = errorMessages;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            LoginResponseDto loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = "Something wrong when login";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

    }
}
