using FucoMicro.Web.Models;

namespace FucoMicro.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> GetAllOrdersAsync(string? userId);
        Task<ResponseDto?> GetOrderByIdAsync(int orderId);
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateSessionStripe(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
