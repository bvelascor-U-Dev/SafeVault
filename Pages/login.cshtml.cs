using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;

[EnableRateLimiting("LoginRateLimit")]
public class LoginModel : PageModel
{
    private readonly SafeValueDbContext _dbContext;
    public string? InformationMessage { get; set; }
    public LoginModel(SafeValueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [BindProperty]
    public LoginUser LoginUser { get; set; } = new();
 
    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Email == LoginUser.Email);
        bool validPassword =
            user is not null &&
            BCrypt.Net.BCrypt.Verify(LoginUser.Password, user.PasswordHash);

        if (!validPassword)
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user!.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToPage("/Members");
    }

    public void OnGet(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            InformationMessage =
                "Debes iniciar sesión para acceder a esa página.";
        }
    }
}
