using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class LoginModel : PageModel
{
    private readonly SafeValueDbContext _dbContext;
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

        var user = _dbContext.Users.FirstOrDefault(u=> u.Email == LoginUser.Email);
        if(user != null)
        {
            if(BCrypt.Net.BCrypt.Verify(LoginUser.Password, user.PasswordHash))
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToPage("/Privacy");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                return Page();
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            return Page();
        }




        return RedirectToPage("/Index");
    }
}