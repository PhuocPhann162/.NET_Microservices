using AutoMapper;
using FucoMicro.Services.ProductAPI.Models;
using FucoMicro.Services.ProductAPI.Models.Dto;

namespace FucoMicro.Services.ProductAPI.Utility
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
