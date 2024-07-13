using FucoMicro.Services.OrderAPI.Models.Dto;

namespace FucoMicro.Services.OrderAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
