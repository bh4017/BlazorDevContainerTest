using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;


public class LogoutModel : PageModel
{
    public IActionResult OnGet() =>
        SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "oidc");
}
