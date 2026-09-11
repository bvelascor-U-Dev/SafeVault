using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? EncryptedPrivateNote { get; set; }
}