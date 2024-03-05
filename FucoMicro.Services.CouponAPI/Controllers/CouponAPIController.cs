using FucoMicro.Services.CouponAPI.Data;
using FucoMicro.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FucoMicro.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CouponAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllCoupon()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                return Ok(objList);
            }
            catch(Exception ex)
            {

            }
            return NotFound("Something wrong when get all coupons");
        }
    }
}
