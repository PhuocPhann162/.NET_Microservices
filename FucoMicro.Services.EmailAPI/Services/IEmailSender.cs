using FucoMicro.Services.EmailAPI.Models.Dto;

namespace FucoMicro.Services.EmailAPI.Services
{
    public interface IEmailSender
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
