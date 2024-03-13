using FucoMicro.Services.ShoppingCartAPI.Models.Dto;

namespace FucoMicro.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
