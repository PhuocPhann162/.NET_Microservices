using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem() { Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem() { Text = SD.RoleCustomer, Value = SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new();
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
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _authService.RegisterAsync(model);
                ResponseDto? assignRole;
                if (response != null && response.IsSuccess)
                {
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = SD.RoleCustomer;
                    }
                    assignRole = await _authService.AssignRoleAsync(model);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = response?.Message;
                        return RedirectToAction(nameof(Login));
                    }
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem() { Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem() { Text = SD.RoleCustomer, Value = SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
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
