using System.Net;

namespace FucoMicro.Web.Models
{
    public class ResponseDto
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = "";
        public object? Result { get; set; }
    }
}
