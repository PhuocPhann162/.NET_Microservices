using FucoMicro.Services.AuthAPI.Data;
using FucoMicro.Services.AuthAPI.Models;
using FucoMicro.Services.AuthAPI.Models.Dto;
using FucoMicro.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace FucoMicro.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.UserName,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber,
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, loginRequestDto.Password);

            if (userFromDb == null && !isValid)
            {
                return new LoginResponseDto() { User = null, Token = "" };

            }

            // if user was found, Generate JWT Tokens
            var token = _jwtTokenGenerator.GenerateToken(userFromDb);


            UserDto userDto = new()
            {
                ID = userFromDb.Id,
                Name = userFromDb.Name,
                Email = userFromDb.Email,
                PhoneNumber = userFromDb.PhoneNumber,
            };

            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token,
            };

            return loginResponseDto;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if(userFromDb != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // create role if it does not exist 
                   await  _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(userFromDb, roleName);
                return true;
            }
            return false;
        }
    }
}
