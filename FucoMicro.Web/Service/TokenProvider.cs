﻿using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;

namespace FucoMicro.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
        }

        public string? GetToken()
        {
            string? token = null;

            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token); 
           
            return hasToken is true ? token : null;
        }

        public void ClearToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }


    }
}
