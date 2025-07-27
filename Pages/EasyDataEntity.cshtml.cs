using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniCodeProject.API.Pages;

[Authorize(Roles = "Administrator")]
public class EasyDataEntityModel : PageModel
{
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostLogoutAsync()
    {
        Console.WriteLine("Logging out...");
        await HttpContext.SignOutAsync("AdminScheme");
        return Redirect("/admin/login");
    }

}