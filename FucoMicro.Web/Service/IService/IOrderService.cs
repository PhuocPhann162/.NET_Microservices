using FucoMicro.Web.Models;

namespace FucoMicro.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
    }
}
