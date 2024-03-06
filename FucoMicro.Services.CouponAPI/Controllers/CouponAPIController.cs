using AutoMapper;
using FucoMicro.Services.CouponAPI.Data;
using FucoMicro.Services.CouponAPI.Models;
using FucoMicro.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FucoMicro.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public CouponAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet("getAll")]
        public ResponseDto GetAll()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Get all coupons successfully";
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto GetById(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.FirstOrDefault(u => u.CouponId == id);
                if(obj == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Not found coupon with id = " + id;
                    return _response;
                }
                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Get coupon by id successfully";
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Not found coupon with code = " + code;
                    return _response;
                }
                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Get coupon by code successfully";
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Create new coupon successfully";
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Update coupon successfully";
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Delete coupon successfully";
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
