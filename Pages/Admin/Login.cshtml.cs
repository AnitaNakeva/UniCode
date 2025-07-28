using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Pages.Admin
{
    public class AdminLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminLoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(Username);
            if (user == null)
            {
                ErrorMessage = "User not found.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            if (!isAdmin)
            {
                ErrorMessage = "You are not an administrator.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var identity = new ClaimsIdentity(claims, "AdminScheme");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AdminScheme", principal);
            
            return Redirect("/easydata");
        }

        public void OnGet()
        {
        }
    }
}