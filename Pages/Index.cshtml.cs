using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages;

public class IndexModel : PageModel
{
    public readonly SafeValueDbContext _dbContext;
    private readonly EncryptionService _encryptionService;
    public IndexModel(SafeValueDbContext dbContext, EncryptionService encryptionService)
    {
        _dbContext = dbContext;
        _encryptionService = encryptionService;
    }

    [BindProperty]
    public RegisterUser registerUser { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPostSubmit()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string username = registerUser.Name;
        string email = registerUser.Email;
        string password = BCrypt.Net.BCrypt.HashPassword(registerUser.PasswordHash);
        string? privateNote = registerUser.PrivateNote != null ? _encryptionService.Encrypt(registerUser.PrivateNote) : null;

        try
        {
            _dbContext.Users.Add(new User { Name = username, Email = email, PasswordHash = password, EncryptedPrivateNote = privateNote });
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
