using AutoMapper;
using FucoMicro.Services.ProductAPI.Data;
using FucoMicro.Services.ProductAPI.Models;
using FucoMicro.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FucoMicro.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db; 
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public ProductAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto GetAll()
        {
            try
            {
                IEnumerable<Product> lstProduct = _db.Products.ToList();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Get all products successfully";
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(lstProduct);
                return _response;
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
                Product? obj = _db.Products.FirstOrDefault(u => u.ProductId == id);
                if(obj == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<ProductDto>(obj);
                _response.Message = "Get product by id successfully";
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
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post(ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                if(productDto.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value} {HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
                _response.Message = "New Product created successfully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);

                if (productDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value} {HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }

                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Product updated successfully";
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
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                if(!string.IsNullOrEmpty(obj.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if(file.Exists)
                    {
                        file.Delete();
                    }
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Product deleted successfully";
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
