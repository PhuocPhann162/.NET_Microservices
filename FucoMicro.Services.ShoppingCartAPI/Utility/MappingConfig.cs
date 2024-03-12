using AutoMapper;
using FucoMicro.Services.ShoppingCartAPI.Models;
using FucoMicro.Services.ShoppingCartAPI.Models.Dto;

namespace FucoMicro.Services.ShoppingCartAPI.Utility
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
