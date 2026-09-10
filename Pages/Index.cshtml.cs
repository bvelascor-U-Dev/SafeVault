using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages;

public class IndexModel : PageModel
{
    public readonly SafeValueDbContext _dbContext;
    public IndexModel(SafeValueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [BindProperty]
    public User user { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPostSubmit()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string username = user.Name;
        string email = user.Email;

        try
        {
            _dbContext.Users.Add(new User { Name = username, Email = email });
            _dbContext.SaveChanges();
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Error al guardar los datos en la base de datos: ");
            return Page();
        }
        
        return RedirectToPage();
    }
}
