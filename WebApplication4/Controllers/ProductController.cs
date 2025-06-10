using BLL.DTOs;
using BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    [ApiController] // Indicates that the class is an API controller
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/Product)
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService; // Dependency injection for ProductService
        public ProductController(ProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products); // Return 200 OK with the list of products
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpGet("GetProductById/{productId}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product ID cannot be empty." });
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound(new ResponseDTO { Success = false, Message = "Product not found." });
                }
                return Ok(product); // Return 200 OK with the product details
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productRequest)
        {
            if (productRequest == null)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product data cannot be null." });
            }

            try
            {
                var response = await _productService.CreateProductAsync(productRequest);
                if (response.Success)
                {
                    return StatusCode(200, new ResponseDTO { Success = true, Message = "Created successfully"}); // Return 201 Created with the new product
                }
                return BadRequest(response); // Return 400 Bad Request with error message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpPut("UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] ProductDTO productRequest)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product ID cannot be empty." });
            }

            if (productRequest == null)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product data cannot be null." });
            }

            try
            {
                var response = await _productService.UpdateProductAsync(productId, productRequest);
                if (response.Success)
                {
                    return Ok(response); // Return 200 OK with success message
                }
                return NotFound(response); // Return 404 Not Found with error message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product ID cannot be empty." });
            }

            try
            {
                var response = await _productService.DeleteProductAsync(productId);
                if (response.Success)
                {
                    return Ok(response); // Return 200 OK with success message
                }
                return NotFound(response); // Return 404 Not Found with error message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

    }
}
