using FluentAssertions;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests;

/// <summary>
/// Unit tests for complex validation scenarios and integration tests
/// </summary>
public class ComplexValidationTests
{
    public class TestUser
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public List<string> Tags { get; set; } = new();
        public TestAddress? Address { get; set; }
    }
    
    public class TestAddress
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";
    }
    
    [Fact]
    public void ArrayValidator_WithValidStrings_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Array(Schema.String().MinLength(2));
        var testData = new[] { "Hello", "World", "Test" };
        
        // Act
        var result = validator.Validate(testData);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ArrayValidator_WithInvalidElements_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Array(Schema.String().MinLength(5));
        var testData = new[] { "Hello", "Hi", "World" };
        
        // Act
        var result = validator.Validate(testData);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Element at index 1: String must be at least 5 characters long");
    }
    
    [Fact]
    public void ArrayValidator_WithLength_ShouldValidateCorrectly()
    {
        // Arrange
        var validator = Schema.Array(Schema.String()).MinLength(2).MaxLength(5);
        var testData = new[] { "One", "Two", "Three" };
        
        // Act
        var result = validator.Validate(testData);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ArrayValidator_TooFewElements_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Array(Schema.String()).MinLength(3);
        var testData = new[] { "One", "Two" };
        
        // Act
        var result = validator.Validate(testData);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Array must contain at least 3 elements");
    }
    
    [Fact]
    public void ArrayValidator_TooManyElements_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Array(Schema.String()).MaxLength(2);
        var testData = new[] { "One", "Two", "Three" };
        
        // Act
        var result = validator.Validate(testData);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Array must contain no more than 2 elements");
    }
    
    [Fact]
    public void ObjectValidator_WithValidObject_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Object<TestUser>()
            .Property("Name", Schema.String().MinLength(2))
            .Property("Email", Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            .Property("Age", Schema.Number().Min(0).Max(150))
            .Property("IsActive", Schema.Boolean());
        
        var testUser = new TestUser
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30,
            IsActive = true
        };
        
        // Act
        var result = validator.Validate(testUser);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ObjectValidator_WithInvalidProperties_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Object<TestUser>()
            .Property("Name", Schema.String().MinLength(5))
            .Property("Email", Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            .Property("Age", Schema.Number().Min(0).Max(150));
        
        var testUser = new TestUser
        {
            Name = "Jo", // Too short
            Email = "invalid-email", // Invalid format
            Age = -5 // Below minimum
        };
        
        // Act
        var result = validator.Validate(testUser);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(0);
        result.Errors.Should().Contain(error => error.Contains("Name"));
        result.Errors.Should().Contain(error => error.Contains("Email"));
        result.Errors.Should().Contain(error => error.Contains("Age"));
    }
    
    [Fact]
    public void ComplexUserValidation_WithNestedObject_ShouldWork()
    {
        // Arrange
        var addressValidator = Schema.Object<TestAddress>()
            .Property("Street", Schema.String().MinLength(5))
            .Property("City", Schema.String().MinLength(2))
            .Property("PostalCode", Schema.String().Pattern(@"^\d{5}$"))
            .Property("Country", Schema.String().MinLength(2));
        
        var userValidator = Schema.Object<TestUser>()
            .Property("Name", Schema.String().MinLength(2).MaxLength(50))
            .Property("Email", Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            .Property("Age", Schema.Number().Min(0).Max(150).Integer())
            .Property("IsActive", Schema.Boolean())
            .Property("Tags", Schema.Array(Schema.String().MinLength(1)).MinLength(1))
            .Property("Address", addressValidator.Optional());
        
        var validUser = new TestUser
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30,
            IsActive = true,
            Tags = new List<string> { "developer", "designer" },
            Address = new TestAddress
            {
                Street = "123 Main St",
                City = "Anytown",
                PostalCode = "12345",
                Country = "USA"
            }
        };
        
        // Act
        var result = userValidator.Validate(validUser);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void EmailValidation_WithVariousFormats_ShouldWorkCorrectly()
    {
        // Arrange
        var validator = Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        
        var validEmails = new[]
        {
            "user@example.com",
            "test.email@domain.org",
            "user+tag@example.co.uk",
            "123@456.789"
        };
        
        var invalidEmails = new[]
        {
            "invalid-email",
            "@example.com",
            "user@",
            "user@domain",
            "user name@example.com",
            ""
        };
        
        // Act & Assert
        foreach (var email in validEmails)
        {
            var result = validator.Validate(email);
            result.IsValid.Should().BeTrue($"Email '{email}' should be valid");
        }
        
        foreach (var email in invalidEmails)
        {
            var result = validator.Validate(email);
            result.IsValid.Should().BeFalse($"Email '{email}' should be invalid");
        }
    }
    
    [Fact]
    public void PhoneNumberValidation_WithPattern_ShouldWork()
    {
        // Arrange
        var validator = Schema.String()
            .Pattern(@"^\+?1?[-.\s]?\(?[0-9]{3}\)?[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$")
            .WithMessage("Invalid phone number format");
        
        var validPhones = new[]
        {
            "123-456-7890",
            "(123) 456-7890",
            "123.456.7890",
            "1234567890",
            "+1-123-456-7890"
        };
        
        // Act & Assert
        foreach (var phone in validPhones)
        {
            var result = validator.Validate(phone);
            result.IsValid.Should().BeTrue($"Phone '{phone}' should be valid");
        }
    }
    
    [Fact]
    public void AgeValidation_WithDifferentScenarios_ShouldWork()
    {
        // Arrange
        var childValidator = Schema.Number().Min(0).Max(17).Integer();
        var adultValidator = Schema.Number().Min(18).Max(120).Integer();
        
        // Act & Assert
        childValidator.Validate(10).IsValid.Should().BeTrue();
        childValidator.Validate(17).IsValid.Should().BeTrue();
        childValidator.Validate(18).IsValid.Should().BeFalse();
        
        adultValidator.Validate(17).IsValid.Should().BeFalse();
        adultValidator.Validate(18).IsValid.Should().BeTrue();
        adultValidator.Validate(65).IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void PasswordValidation_WithComplexRules_ShouldWork()
    {
        // Arrange
        var validator = Schema.String()
            .MinLength(8)
            .MaxLength(50)
            .Pattern(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");
        
        var validPasswords = new[]
        {
            "Password123!",
            "MySecure@Pass1",
            "ComplexP@ssw0rd"
        };
        
        var invalidPasswords = new[]
        {
            "password", // No uppercase, number, or special char
            "PASSWORD123", // No lowercase or special char
            "Pass123", // Too short
            "Password123" // No special char
        };
        
        // Act & Assert
        foreach (var password in validPasswords)
        {
            var result = validator.Validate(password);
            result.IsValid.Should().BeTrue($"Password '{password}' should be valid");
        }
        
        foreach (var password in invalidPasswords)
        {
            var result = validator.Validate(password);
            result.IsValid.Should().BeFalse($"Password '{password}' should be invalid");
        }
    }
} 