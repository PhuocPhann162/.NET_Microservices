using FucoMicro.Services.EmailAPI.Models.Dto;

namespace FucoMicro.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
