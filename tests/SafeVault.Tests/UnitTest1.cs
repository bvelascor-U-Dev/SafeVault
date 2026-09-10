using System.ComponentModel.DataAnnotations;
namespace SafeVault.Tests;

public class UnitTest1
{
    [Fact]
    public void userWithDataValidIsvalid()
    { 
        //Arrange
        var user = new User
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

                // Act: ejecutamos las validaciones del modelo
        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert: comprobamos el resultado esperado
        Assert.True(isValid);
    }
    [Fact]
    public void UserWithSqlInjectionIsInvalid()
    {
        // Arrange
        var user = new User
        {
            Name = "Pedro'); DROP TABLE Users;--",
            Email = "pedro@example.com"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);
    }
    [Fact]
    public void UserWithXssScriptIsInvalid()
    {
        // Arrange
        var user = new User
        {
            Name = "<script>alert('XSS')</script>",
            Email = "pedro@example.com"
        };

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(
            user,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);
    }
}
