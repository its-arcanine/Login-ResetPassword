using BLL.DTOs;
using BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CartController : Controller
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartItemByCartId(string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                return BadRequest("Cart ID cannot be null or empty.");
            }

            var cart = await _cartService.GetCartItemByCartIdAsync(cartId);
            if (cart == null)
            {
                return NotFound($"Cart with ID {cartId} not found.");
            }

            return Ok(cart);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCart( string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest("Account ID cannot be null or empty.");
            }

            var response = await _cartService.CreateCartAsync(accountId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return StatusCode(200, new ResponseDTO { Success = true, Message = response.Message});
        }
        [HttpPost("add-product/{cartId}/{productId}")]
        public async Task<IActionResult> AddProduct(string cartId, string productId, int quantity)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(productId))
            {
                return BadRequest("Cart ID and Product ID cannot be null or empty.");
            }

            var response = await _cartService.AddProduct(cartId, productId, quantity);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
        [HttpPost("remove-product/{cartId}/{cartItemId}")]
        public async Task<IActionResult> RemoveProduct(string cartId, string cartItemId)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(cartItemId))
            {
                return BadRequest("Cart ID and Product ID cannot be null or empty.");
            }

            var response = await _cartService.RemoveProduct(cartId, cartItemId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }
        
    }
}
