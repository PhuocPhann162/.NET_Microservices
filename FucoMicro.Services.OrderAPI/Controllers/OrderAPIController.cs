﻿using AutoMapper;
using Azure;
using FucoMicro.Services.OrderAPI.Data;
using FucoMicro.Services.OrderAPI.Models;
using FucoMicro.Services.OrderAPI.Models.Dto;
using FucoMicro.Services.OrderAPI.Services;
using FucoMicro.Services.OrderAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FucoMicro.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly ApplicationDbContext _db;
        private IProductService _productService;

        public OrderAPIController(ApplicationDbContext db, IProductService productService, IMapper mapper)
        {
            _db = db;
            _productService = productService;
            _mapper = mapper;
            this._response = new();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                if (cartDto == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.Message = "Something wrong. Your cart may not be found!";
                    return _response;
                }
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;

                _response.Result = orderHeaderDto;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Create new order successfully";
                return _response;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}