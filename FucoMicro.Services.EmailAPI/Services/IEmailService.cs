using FucoMicro.Services.EmailAPI.Message;
using FucoMicro.Services.EmailAPI.Models.Dto;

namespace FucoMicro.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);

        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
