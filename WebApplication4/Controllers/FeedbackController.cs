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
    public class FeedbackController : ControllerBase
    {
       private readonly FeedbackService _feedbackService;
       public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        [HttpPost("SubmitFeedback")]
        public async Task<IActionResult> SubmitFeedback(string productId, [FromBody] FeedbackDTO feedbackDTO)
        {
            if (string.IsNullOrEmpty(productId) && feedbackDTO.Comment != null)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product ID & review cannot be empty." });
            }
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the account ID from the authenticated user
            var feedbackRequest = new FeedbackRequestDTO
            {
                AccountId = accountId, // Use the account ID from the authenticated user
                ProductId = productId,
                Comment = feedbackDTO.Comment,
                Rating = feedbackDTO.Rating,
            };
            var response = await _feedbackService.CreateFeedback(feedbackRequest);
            if (response.Success)
            {
                return Ok(response); // Return 200 OK with success message
            }
            else
            {
                return BadRequest(response); // Return 400 Bad Request with error message
            }
        }


        [HttpGet("GetFeedbackById/{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(string feedbackId)
        {
            if (string.IsNullOrEmpty(feedbackId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Feedback ID cannot be empty." });
            }

            var feedback = await _feedbackService.GetFeedbackByIdAsync(feedbackId);
            if (feedback == null)
            {
                return NotFound(new ResponseDTO { Success = false, Message = "Feedback not found." });
            }

            return Ok(feedback); // Return 200 OK with the feedback details
        }

        [HttpGet("GetAllFeedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Ok(feedbacks); // Return 200 OK with the list of feedbacks
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO { Success = false, Message = ex.Message }); // Return 500 Internal Server Error
            }
        }


        [HttpGet("GetFeedbackByProductId/{productId}")]
        public async Task<IActionResult> GetFeedbackByProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Product ID cannot be empty." });
            }

            var feedbacks = await _feedbackService.GetFeedbackByProductIdAsync(productId);
            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound(new ResponseDTO { Success = false, Message = "No feedback found for this product." });
            }

            return Ok(feedbacks); // Return 200 OK with the list of feedbacks for the product
        }

    }
}
