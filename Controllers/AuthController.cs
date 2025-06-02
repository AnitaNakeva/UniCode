using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.DataModels;
using UniCodeProject.API.DTOs;
using UniCodeProject.API.Services;

namespace UniCodeProject.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IAuthService _service;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, JwtTokenService jwtTokenService, IAuthService service, IEmailService emailService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _service = service;
            _emailService = emailService;
        }

        // POST: api/auth/token
        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = _jwtTokenService.GenerateToken(user);
                return Ok(new { token });
            }



            return Unauthorized(new { message = "Invalid email or password." });
        }


        [HttpPost("/api/users")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string roleName = await _service.CheckRole(model.Email);
            if (roleName == "no role")
            {
                return BadRequest(new { message = "Invalid email domain. Registration is only allowed for students and teachers." });
            }

            var user = new ApplicationUser
            {
                UserName = model.FirstName + model.LastName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                string university = await _service.CheckUniversity(user, roleName);

                if (university == "No university")
                {
                    return BadRequest(new { message = "Invalid email domain. Registration is only allowed for students and teachers." });
                }

                await _service.CreateProfile(user, roleName, university);

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    return BadRequest("User email is required.");
                }

                // Send the registration email
                await _emailService.SendRegistrationEmailAsync(user.Email, user.Id);

                return CreatedAtAction(nameof(Login), new { email = user.Email }, new { message = "Registration successful. Please check your email to confirm your account.", role = roleName });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "User logged out successfully. Please delete the token on client side." });
        }

    }
}