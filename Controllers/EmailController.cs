using Microsoft.AspNetCore.Mvc;
using UniCodeProject.API.Contracts;

namespace UniCodeProject.API.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-registration-email")]
        public async Task<IActionResult> SendRegistrationEmail(string email, string userId)
        {
            await _emailService.SendRegistrationEmailAsync(email, userId);
            return Ok("Registration email sent.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            // Call ConfirmEmailAsync in the service
            var result = await _emailService.ConfirmEmailAsync(userId, token);

            if (!result)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Email confirmed successfully.");
        }

    }
}
