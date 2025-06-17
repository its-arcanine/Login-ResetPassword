using BLL.DTOs;
using BLL.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security;
using System.Text;

namespace WebApplication4.Controllers
{
    [ApiController] // Indicates that the class is an API controller
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/Account)
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService; // Dependency injection for AccountService
        private readonly IConfiguration _configuration; // Configuration for accessing app settings
        /// <summary>
        /// Constructor for AccountController.
        /// Injects the AccountService dependency.
        /// </summary>
        /// <param name="accountService">The service responsible for account operations.</param>
        public AccountController(AccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("SendRegisterEmail")]
        public async Task<IActionResult> SendRegisterEmail([FromBody] string email)
        {
            // Validate the incoming request model
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Email cannot be empty." });
            }

            // Call the service to send the registration email
            var response = await _accountService.RequestCreateAccountAsync(email);

            // Check the success status from the service response
            if (response.Success)
            {
                return Ok(response); // Return 200 OK with success message
            }
            else
            {
                return BadRequest(response); // Return 400 Bad Request with error message
            }
        }
        [HttpPost("VerifyRegister")]
        public async Task<IActionResult> VerifyRegister(string verificationToken, [FromBody] AccountCreateRequestDTO accountCreateRequest)
        {
            if (string.IsNullOrWhiteSpace(verificationToken))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Email cannot be empty." });
            }
            verificationToken = verificationToken.Trim(); // Trim whitespace from the token
            var response = await _accountService.CreateAccountAsync(verificationToken, accountCreateRequest);
            if (response.Success)
            {
                return Ok(response); // Return 200 OK with success message
            }
            else
            {
                return BadRequest(response); // Return 400 Bad Request with error message
            }
        }

        /*  public async Task<IActionResult> HandleToken(string verificationToken)
          {
              if (string.IsNullOrWhiteSpace(verificationToken))
              {
                  return BadRequest(new ResponseDTO { Success = false, Message = "Email cannot be empty." });
              }
              verficationToken = verficationToken.Trim(); // Trim whitespace from the token

           }*/


        /// <summary>
        /// Handles user login.
        /// Responds to HTTP POST requests at /api/Account/login.
        /// </summary>
        /// <param name="accountRequest">Login credentials (email and password) provided in the request body.</param>
        /// <returns>An IActionResult indicating the success or failure of the login attempt.</returns>
        [HttpPost("login")] // Defines a specific route for this action (e.g., /api/Account/login)
        public async Task<IActionResult> LoginAccount([FromBody] AccountRequestDTO accountRequest)
        {
            // Validate the incoming request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model validation fails
            }

            // Call the service to authenticate the account
            var response = await _accountService.LoginAccountAsync(accountRequest);

            // Check the success status from the service response
            if (response.Success)
            {
               
                return Ok(response); // Return 200 OK with success message
            }
            else
            {

                return Unauthorized(response); // Return 401 Unauthorized for invalid credentials
            }
        }
        [HttpPost("forgot-password")] // POST /api/account/forgot-password
        public async Task<IActionResult> ForgotPassword([FromBody] string requestEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Delegate to the service layer.
            // The service layer handles whether the email actually exists
            // and sends the email if it does.
            var response = await _accountService.RequestPasswordResetAsync(requestEmail);

            // Always return a generic success message for security reasons
            // to prevent attackers from determining valid email addresses.
            if (response.Success)
            {
                return Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
            }
            else
            {
                // This case indicates an internal server error in sending the email,
                // not that the email address was invalid.
                return StatusCode(500, new { Message = "An error occurred while trying to send the reset email. Please try again later." });
            }
        }

        [HttpPost("reset-password")] // POST /api/account/reset-password
        public async Task<IActionResult> ResetPassword(string requestToken, string requestPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Delegate to the service layer
            var response = await _accountService.ResetPasswordAsync(requestToken, requestPassword);

            if (response.Success)
            {
                return Ok(new { Message = "Password has been successfully reset." });
            }
            else
            {
                // Use the error message from the service, which should be generic for security
                return BadRequest(new { Message = "LMAO" });
            }
        }



        /// <summary>
        /// Retrieves account details by email.
        /// Responds to HTTP GET requests at /api/Account/{email}.
        /// </summary>
        /// <param name="email">The email of the account to retrieve, provided in the URL path.</param>
        /// <returns>An IActionResult containing the account data if found, or an error message if not.</returns>
        [HttpGet("{email}")] // Defines a route with a parameter (e.g., /api/Account/test@example.com)
        public async Task<IActionResult> GetAccountByEmail(string email)
        {
            // Basic validation for email parameter
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Email cannot be empty." });
            }

            // Call the service to get account by email
            var response = await _accountService.GetAccountByEmailAsync(email);

            // Check the success status from the service response
            if (response.Success)
            {
                return Ok(response); // Return 200 OK with the account data
            }
            else
            {
                return NotFound(response); // Return 404 Not Found if account is not found
            }
        }
    }

}

