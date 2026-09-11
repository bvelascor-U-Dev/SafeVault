using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class MembersModel : PageModel
{
    private readonly SafeValueDbContext _dbContext;
    private readonly EncryptionService _encryptionService;
    public string? DecryptedPrivateNote { get; private set; }

    public MembersModel(SafeValueDbContext dbContext, EncryptionService encryptionService)
    {
        _dbContext = dbContext;
        _encryptionService = encryptionService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail != null)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u=> u.Email == userEmail);
            if (user != null && user.EncryptedPrivateNote != null)
            {
                DecryptedPrivateNote = _encryptionService.Decrypt(user.EncryptedPrivateNote);
            }
        } else
        {
            ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            return Challenge();
        }

        return Page();
    }
}
