using FucoMicro.Services.ShoppingCartAPI.Models.Dto;
using FucoMicro.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace FucoMicro.Services.ShoppingCartAPI.Services
{
    public class ProductService : IProductService
    {
        private IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var responseJson = await client.GetAsync($"/api/product");
            var apiContent = await responseJson.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if(response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(response.Result));
            }

            return new List<ProductDto>();
        }
    }
}
