using System.ComponentModel.DataAnnotations;
public class RegisterUser
{
    [Required]
    [RegularExpression
        (@"^[\p{L} ]+$",
        ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [RegularExpression
        (@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, incluyendo una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
    public string PasswordHash { get; set; } = string.Empty;
    public string? PrivateNote { get; set; } = string.Empty;
}