using FucoMicro.Web.Models;

namespace FucoMicro.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateSessionStripe(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
    }
}
