using AutoMapper;
using FucoMicro.Services.CouponAPI.Models;
using FucoMicro.Services.CouponAPI.Models.Dto;

namespace FucoMicro.Services.CouponAPI.Utility
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
