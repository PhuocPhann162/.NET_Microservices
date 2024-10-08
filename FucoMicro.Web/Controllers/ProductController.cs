﻿using FucoMicro.Web.Models;
using FucoMicro.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FucoMicro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();
            ResponseDto? response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = response?.Message;
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(int productId)
        {
            if (ModelState.IsValid)
            {

                if (productId == null || productId == 0)
                {
                    return NotFound();
                }
                ResponseDto? response = await _productService.GetProductByIdAsync(productId);
                if (response != null && response.IsSuccess)
                {
                    ProductDto? product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                    return View(product);
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = response?.Message;
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            if (productId == null || productId == 0)
            {
                return NotFound();
            }
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto? product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(product);
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = response?.Message;
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

    }
}
