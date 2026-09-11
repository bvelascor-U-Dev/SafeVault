using System.ComponentModel.DataAnnotations;

namespace SafeVault.Tests;

public class RegistrationModelTests
{
    [Fact]
    public void RegisterUserWithValidDataIsValid()
    {
        var user = new RegisterUser
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            PasswordHash = "Password1!"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        Assert.True(isValid);
    }

    [Fact]
    public void RegisterUserWithSqlInjectionInNameIsInvalid()
    {
        var user = new RegisterUser
        {
            Name = "Pedro'); DROP TABLE Users;--",
            Email = "pedro@example.com",
            PasswordHash = "Password1!"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
    }

    [Fact]
    public void RegisterUserWithXssInNameIsInvalid()
    {
        var user = new RegisterUser
        {
            Name = "<script>alert('XSS')</script>",
            Email = "pedro@example.com",
            PasswordHash = "Password1!"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
    }
}
