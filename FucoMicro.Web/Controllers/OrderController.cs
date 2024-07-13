using Microsoft.AspNetCore.Mvc;

namespace FucoMicro.Web.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
