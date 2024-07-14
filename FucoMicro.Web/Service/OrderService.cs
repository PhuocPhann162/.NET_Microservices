using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;

namespace FucoMicro.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST, 
                Data = cartDto, 
                Url = SD.OrderAPIBase + "/api/order/CreateOrder",
            });
        }
    }
}
