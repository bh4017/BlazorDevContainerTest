using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

public class LoginModel : PageModel
{
    public IActionResult OnGet() =>
        Challenge(new AuthenticationProperties { RedirectUri = "/" }, "oidc");
}
