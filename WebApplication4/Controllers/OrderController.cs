using BLL.DTOs;
using BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApplication4.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("GetOrderById/{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Order ID cannot be empty." });
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new ResponseDTO { Success = false, Message = "Order not found." });
                }
                return Ok(order); // Return 200 OK with the order details
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders); // Return 200 OK with the list of orders
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }


        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(string productId, [FromBody] string address, int quantity, int total)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(address) || quantity <= 0 || total <= 0)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Invalid order data." });
            }
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = new OrderDTO
            {
                AccountId = accountId,
                ProductId = productId,
                Address = address,
                Quantity = quantity,
                TotalAmount = total,
            };
            try
            {
                var response = await _orderService.CreateOrderAsync(order);
                if (response.Success)
                {
                    return Ok(response); // Return 200 OK with success message
                }
                else
                {
                    return BadRequest(response); // Return 400 Bad Request with error message
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }


        [HttpGet("GetOrderByAccountId/{accountId}")]
        public async Task<IActionResult> GetOrderByAccountId(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Account ID cannot be empty." });
            }

            try
            {
                var response = await _orderService.GetOrderByAccountIdAsync(accountId);
                if (!response.Success)
                {
                    return NotFound(response); // Return 404 Not Found with error message
                }
                return Ok(response); // Return 200 OK with the list of orders for the account
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpGet("FilterDateOrder")]
        public async Task<IActionResult> FilterDateOrder(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Start date and end date cannot be empty." });
            }

            try
            {
                var response = await _orderService.FilterDateOrder(startDate, endDate);
                if (!response.Success)
                {
                    return NotFound(response); // Return 404 Not Found with error message
                }
                return Ok(response); // Return 200 OK with the list of orders in the specified date range
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }

        [HttpGet("SumAllOrder")]
        public async Task<IActionResult> SumAllOrder(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Start date and end date cannot be empty." });
            }

            try
            {
                var response = await _orderService.SumAllOrder(startDate, endDate);
                if (!response.Success)
                {
                    return NotFound(response); // Return 404 Not Found with error message
                }
                return Ok(response); // Return 200 OK with the total amount of orders in the specified date range
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }
    }
}
