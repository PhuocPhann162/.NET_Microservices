using AutoMapper;
using Azure;
using FucoMicro.MessageBus;
using FucoMicro.Services.OrderAPI.Data;
using FucoMicro.Services.OrderAPI.Models;
using FucoMicro.Services.OrderAPI.Models.Dto;
using FucoMicro.Services.OrderAPI.Services;
using FucoMicro.Services.OrderAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Net;

namespace FucoMicro.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(ApplicationDbContext db, IProductService productService, IMapper mapper, IConfiguration configuration, IMessageBus messageBus)
        {
            _db = db;
            _productService = productService;
            _mapper = mapper;
            this._response = new();
            _configuration = configuration;
            _messageBus = messageBus;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = await _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToListAsync();
                }
                else
                {
                    objList = await _db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId)
                        .OrderByDescending(u => u.OrderHeaderId).ToListAsync();
                }

                if (objList == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.Message = "Orders not found";
                    return _response;
                }

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
                _response.Message = "Get orders successfully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status500InternalServerError;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrderById/{orderId:int}")]
        public async Task<ResponseDto?> GetOrderById(int orderId)
        {
            try
            {
                OrderHeader? orderFromDb = await _db.OrderHeaders.Include(u => u.OrderDetails).FirstAsync(u => u.OrderHeaderId == orderId);

                if (orderFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.Message = "Not Found ! Order ID: " + orderId + " may not exist";
                    return _response;
                }

                _response.Result = _mapper.Map<OrderHeaderDto>(orderFromDb);
                _response.Message = "Get order by ID: " + orderId + " successfully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = (HttpStatusCode)StatusCodes.Status500InternalServerError;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
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
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.ApprovedUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountObj;
                }

                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;

                // Store stripe to database
                OrderHeader orderHeaderFromDb = await _db.OrderHeaders.FirstAsync(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeaderFromDb.StripeSessionId = session.Id;
                _db.SaveChanges();

                _response.Result = stripeRequestDto;
                _response.Message = "Create Stripe session successfully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeaderFromDb = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new Stripe.Checkout.SessionService();
                Session session = service.Get(orderHeaderFromDb.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    // then payment was successful
                    orderHeaderFromDb.PaymentIntentId = paymentIntent.Id;
                    orderHeaderFromDb.Status = SD.Status_Approved;
                    _db.SaveChanges();

                    RewardsDto rewardsDto = new()
                    {
                        OrderId = orderHeaderFromDb.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeaderFromDb.OrderTotal),
                        UserId = orderHeaderFromDb.UserId
                    };
                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDto, topicName);

                    _response.Result = _mapper.Map<ResponseDto>(orderHeaderFromDb);
                }

                _response.Message = "Create Stripe session successfully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderFromDb = await _db.OrderHeaders.FirstAsync(u => u.OrderHeaderId == orderId);
                if (orderFromDb != null)
                {
                    if (newStatus == SD.Status_Cancelled)
                    {
                        // we will give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderFromDb.PaymentIntentId

                        };
                        var service = new RefundService();
                        Refund refund = service.Create(options);
                        orderFromDb.Status = newStatus;
                    }
                    orderFromDb.Status = newStatus;
                    await _db.SaveChangesAsync();

                    _response.Result = orderFromDb;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Update order status successfully";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
