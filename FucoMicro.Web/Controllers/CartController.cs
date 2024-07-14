using FucoMicro.Web.Models;
using FucoMicro.Web.Service;
using FucoMicro.Web.Service.IService;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace FucoMicro.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBaseOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBaseOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBaseOnLoggedInUser();
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;

            ResponseDto? response = await _orderService.CreateOrderAsync(cart);
            OrderHeaderDto newOrder = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                // get stripe session and redirect to stripe to place order 
            }

            return View();
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                bool check = Convert.ToBoolean(response.Result);
                if (check)
                {
                    TempData["success"] = response.Message;
                    return RedirectToAction(nameof(CartIndex));
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            if (cartDto.CartHeader.CouponCode == null)
            {
                TempData["error"] = "You didn't fill any coupon code";
                return RedirectToAction(nameof(CartIndex));
            }
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                bool check = Convert.ToBoolean(response.Result);
                if (check)
                {
                    TempData["success"] = response.Message;
                    return RedirectToAction(nameof(CartIndex));
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(cartDto);
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBaseOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.EmailCartAsync(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _cartService.RemoveCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                bool check = Convert.ToBoolean(response.Result);
                if (check)
                {
                    TempData["success"] = response.Message;
                    return RedirectToAction(nameof(CartIndex));
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(cartDto);
        }


        private async Task<CartDto> LoadCartDtoBaseOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cart;
            }

            return new CartDto();
        }
    }
}
