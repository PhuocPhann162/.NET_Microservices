using FucoMicro.MessageBus;
using FucoMicro.Services.AuthAPI.Data;
using FucoMicro.Services.AuthAPI.Models.Dto;
using FucoMicro.Services.AuthAPI.RabbitMQSender;
using FucoMicro.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using System.Net;

namespace FucoMicro.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRabbitMQAuthMessageSender _messageBus;
        private readonly IConfiguration _configuration;
        private ResponseDto _response;

        public AuthAPIController(IAuthService authService, IRabbitMQAuthMessageSender messageBus, IConfiguration configuration)
        {
            _response = new ResponseDto();
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
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
            _messageBus.SendMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            _response.Message = "Registration successfully";
            _response.StatusCode = HttpStatusCode.OK;
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
            _response.Message = "Login to account successfully";
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = "Something wrong when assigning user role";
                return BadRequest(_response);
            }
            return Ok(_response); 
        }
    }
}
