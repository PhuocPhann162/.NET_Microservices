using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace FucoMicro.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new ();
            return View(loginRequestDto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? response = await _authService.RegisterAsync(model);
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = response?.Message;
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _authService.LoginAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = response?.Message;
                    return RedirectToAction(nameof(Index), nameof(HomeController));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

    }
}
