using FucoMicro.Services.ShoppingCartAPI.Models.Dto;

namespace FucoMicro.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByCode(string code);
    }
}
