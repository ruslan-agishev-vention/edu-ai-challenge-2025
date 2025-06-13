using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace ValidationLibrary.Tests;

/// <summary>
/// Unit tests for StringValidator class
/// </summary>
public class StringValidatorTests
{
    [Fact]
    public void Validate_ValidString_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String();
        
        // Act
        var result = validator.Validate("Hello World");
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Validate_NullString_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be null");
    }
    
    [Fact]
    public void MinLength_StringTooShort_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().MinLength(5);
        
        // Act
        var result = validator.Validate("Hi");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String must be at least 5 characters long");
    }
    
    [Fact]
    public void MinLength_StringExactLength_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().MinLength(5);
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void MaxLength_StringTooLong_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().MaxLength(5);
        
        // Act
        var result = validator.Validate("Hello World");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String must be no more than 5 characters long");
    }
    
    [Fact]
    public void MaxLength_StringExactLength_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().MaxLength(5);
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Pattern_StringMatches_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().Pattern(@"^\d{3}-\d{3}-\d{4}$");
        
        // Act
        var result = validator.Validate("123-456-7890");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Pattern_StringDoesNotMatch_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().Pattern(@"^\d{3}-\d{3}-\d{4}$");
        
        // Act
        var result = validator.Validate("invalid-phone");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Contains("does not match required pattern"));
    }
    
    [Fact]
    public void Pattern_WithRegexObject_ShouldWork()
    {
        // Arrange
        var regex = new Regex(@"^[A-Z][a-z]+$");
        var validator = Schema.String().Pattern(regex);
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void NotEmpty_WithValidString_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().NotEmpty();
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void NotEmpty_WithEmptyString_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().NotEmpty();
        
        // Act
        var result = validator.Validate("");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String failed custom validation");
    }
    
    [Fact]
    public void NotEmpty_WithWhitespace_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().NotEmpty();
        
        // Act
        var result = validator.Validate("   ");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String failed custom validation");
    }
    
    [Fact]
    public void AlphaNumeric_WithValidString_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().AlphaNumeric();
        
        // Act
        var result = validator.Validate("Hello123");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void AlphaNumeric_WithInvalidString_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().AlphaNumeric();
        
        // Act
        var result = validator.Validate("Hello-123");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String failed custom validation");
    }
    
    [Fact]
    public void ChainedValidations_AllValid_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String()
            .MinLength(5)
            .MaxLength(10)
            .Pattern(@"^[A-Za-z]+$")
            .NotEmpty();
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ChainedValidations_MultipleFailures_ShouldReturnAllErrors()
    {
        // Arrange
        var validator = Schema.String()
            .MinLength(10)
            .MaxLength(5);
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1); // Only min length fails since 5 >= 5
        result.Errors.Should().Contain("String must be at least 10 characters long");
    }
    
    [Fact]
    public void WithMessage_ShouldUseCustomErrorMessage()
    {
        // Arrange
        var validator = Schema.String().MinLength(10).WithMessage("Custom error message");
        
        // Act
        var result = validator.Validate("Short");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Custom error message");
    }
    
    [Fact]
    public void Optional_WithNullValue_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.String().MinLength(5).Optional();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Optional_WithValidValue_ShouldValidateNormally()
    {
        // Arrange
        var validator = Schema.String().MinLength(5).Optional();
        
        // Act
        var result = validator.Validate("Hello");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Optional_WithInvalidValue_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.String().MinLength(5).Optional();
        
        // Act
        var result = validator.Validate("Hi");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("String must be at least 5 characters long");
    }
} 