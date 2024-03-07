using FucoMicro.Services.AuthAPI.Data;
using FucoMicro.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FucoMicro.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _responseDto;

        public AuthAPIController(ApplicationDbContext db)
        {
            _db = db;
            _responseDto = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registerRequestDto )
        {
            try
            {

                Regi
            }
            catch(Exception ex)
            {
                _responseDto.StatusCode = HttpStatusCode.BadRequest;
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _responseDto.StatusCode = HttpStatusCode.BadRequest;
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return Ok();
        }

    }
}
