using FucoMicro.Web.Models;

namespace FucoMicro.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveCouponAsync(CartDto cartDto);
        Task<ResponseDto?> EmailCartAsync(CartDto cartDto);
    }
}
