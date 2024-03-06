using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FucoMicro.Web.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService _couponService;

		public CouponController(ICouponService couponService)
		{
			_couponService = couponService;
		}

		public async Task<IActionResult> CouponIndex()
		{
			List<CouponDto>? list = new();
			ResponseDto? responseDto = await _couponService.GetAllCouponsAsync();
			if (responseDto != null && responseDto.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(responseDto.Result));
			}
			return View(list);
		}

		public async Task<IActionResult> CouponCreate()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CouponCreate(CouponDto couponDto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? responseDto = await _couponService.CreateCouponAsync(couponDto);
				if(responseDto != null && responseDto.IsSuccess)
				{
					return RedirectToAction(nameof(CouponIndex));
				}
			}
			return View(couponDto);
		}

		[HttpDelete]
		public async Task<IActionResult> CouponUpdate(CouponDto couponDto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? responseDto = await _couponService.UpdateCouponAsync(couponDto);
				if (responseDto != null && responseDto.IsSuccess)
				{
					return RedirectToAction(nameof(CouponIndex));
				}
			}
			return View(couponDto);
		}

		public async Task<IActionResult> CouponDelete(int couponId)
		{
			ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);
			if (response != null && response.IsSuccess)
			{
                CouponDto model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);
            }
			return NotFound();
        }

		[HttpPost, ActionName("CouponDelete")]
        public async Task<IActionResult> CouponDeletePost(int couponId)
        {
            ResponseDto? response = await _couponService.DeleteCouponAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            return View(couponId);
        }
    }
}
