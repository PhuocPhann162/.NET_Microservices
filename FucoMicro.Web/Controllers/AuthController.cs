﻿using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FucoMicro.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
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

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser(RegistrationRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? responseDto = await _authService.RegisterAsync(model);
                ResponseDto? assignRole;
                if (responseDto != null && responseDto.IsSuccess)
                {
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = SD.RoleCustomer;
                    }
                    assignRole = await _authService.AssignRoleAsync(model);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = responseDto?.Message;
                        return RedirectToAction(nameof(RegisterNewUser));
                    }
                }
                else
                {
                    TempData["error"] = responseDto?.Message;
                }
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem() {Text =SD.RoleCustomer, Value = SD.RoleCustomer},
                new SelectListItem() {Text = SD.RoleAdmin, Value = SD.RoleAdmin},
            };
            ViewBag.RoleList = roleList;
            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _authService.LoginAsync(model);
                if (response != null && response.IsSuccess)
                {
                    LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                    await SignInUser(loginResponseDto);
                    _tokenProvider.SetToken(loginResponseDto.Token);
                    TempData["success"] = response?.Message;
                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    TempData["error"] = response?.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            TempData["success"] = "Sign out successfully";
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
