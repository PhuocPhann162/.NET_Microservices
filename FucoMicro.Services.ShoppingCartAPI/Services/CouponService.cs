using FucoMicro.Services.ShoppingCartAPI.Models.Dto;
using FucoMicro.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace FucoMicro.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto> GetCouponByCode(string code)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var responseJson = await client.GetAsync($"/api/coupon/getByCode/{code}");
            var apiContent = await responseJson.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if(response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            }
            return new CouponDto();
        }
    }
}
