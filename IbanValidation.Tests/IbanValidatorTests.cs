namespace IbanValidation.Tests;

public class IbanValidatorTests
{
    [Test]
    public void Validate_Iban_ReturnsTrue()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("NL28RABO3154172025");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Validate_IbanWithSpaces_ReturnsTrue()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("NL28 RABO 3154 1720 25");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Validate_IbanWithSpecialCharacters_ReturnsTrue()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("NL28-RABO-3154-1720-25");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Validate_IbanWithInvalidLength_ReturnsFalse()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("NL28RABO315417202433");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Validate_EmptyIban_ReturnsFalse()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Validate_InvalidIban_ReturnsFalse()
    {
        // Arrange
        var ibanValidator = new IbanValidator();

        // Act
        var result = ibanValidator.Validate("NL28RABO315417312433");

        // Assert
        Assert.That(result, Is.False);
    }
}