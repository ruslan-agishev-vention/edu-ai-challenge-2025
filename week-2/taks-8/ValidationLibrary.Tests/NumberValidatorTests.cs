using FluentAssertions;
using Xunit;

namespace ValidationLibrary.Tests;

/// <summary>
/// Unit tests for NumberValidator class
/// </summary>
public class NumberValidatorTests
{
    [Fact]
    public void Validate_ValidNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number();
        
        // Act
        var result = validator.Validate(42.5);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Validate_NullNumber_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be null");
    }
    
    [Fact]
    public void Min_NumberTooSmall_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Min(10);
        
        // Act
        var result = validator.Validate(5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be greater than or equal to 10");
    }
    
    [Fact]
    public void Min_NumberAtMinimum_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Min(10);
        
        // Act
        var result = validator.Validate(10);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Min_ExclusiveMinimum_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Min(10, inclusive: false);
        
        // Act
        var result = validator.Validate(10);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be greater than 10");
    }
    
    [Fact]
    public void Max_NumberTooLarge_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Max(10);
        
        // Act
        var result = validator.Validate(15);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be less than or equal to 10");
    }
    
    [Fact]
    public void Max_NumberAtMaximum_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Max(10);
        
        // Act
        var result = validator.Validate(10);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Max_ExclusiveMaximum_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Max(10, inclusive: false);
        
        // Act
        var result = validator.Validate(10);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be less than 10");
    }
    
    [Fact]
    public void Positive_WithPositiveNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Positive();
        
        // Act
        var result = validator.Validate(5.5);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Positive_WithZero_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Positive();
        
        // Act
        var result = validator.Validate(0);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Number failed custom validation");
    }
    
    [Fact]
    public void Positive_WithNegativeNumber_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Positive();
        
        // Act
        var result = validator.Validate(-5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Number failed custom validation");
    }
    
    [Fact]
    public void Negative_WithNegativeNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Negative();
        
        // Act
        var result = validator.Validate(-5.5);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Negative_WithPositiveNumber_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Negative();
        
        // Act
        var result = validator.Validate(5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Number failed custom validation");
    }
    
    [Fact]
    public void Integer_WithWholeNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Integer();
        
        // Act
        var result = validator.Validate(42);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Integer_WithDecimalNumber_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Integer();
        
        // Act
        var result = validator.Validate(42.5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Number failed custom validation");
    }
    
    [Fact]
    public void Validate_WithNaN_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number();
        
        // Act
        var result = validator.Validate(double.NaN);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be NaN");
    }
    
    [Fact]
    public void Validate_WithPositiveInfinity_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number();
        
        // Act
        var result = validator.Validate(double.PositiveInfinity);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be infinite");
    }
    
    [Fact]
    public void Validate_WithNegativeInfinity_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number();
        
        // Act
        var result = validator.Validate(double.NegativeInfinity);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be infinite");
    }
    
    [Fact]
    public void ChainedValidations_AllValid_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number()
            .Min(0)
            .Max(100)
            .Positive();
        
        // Act
        var result = validator.Validate(50);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ChainedValidations_MultipleFailures_ShouldReturnAllErrors()
    {
        // Arrange
        var validator = Schema.Number()
            .Min(50)
            .Max(100);
        
        // Act
        var result = validator.Validate(25);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be greater than or equal to 50");
    }
    
    [Fact]
    public void WithMessage_ShouldUseCustomErrorMessage()
    {
        // Arrange
        var validator = Schema.Number().Min(10).WithMessage("Number is too small");
        
        // Act
        var result = validator.Validate(5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Number is too small");
    }
    
    [Fact]
    public void Optional_WithNullValue_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Min(10).Optional();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Optional_WithValidValue_ShouldValidateNormally()
    {
        // Arrange
        var validator = Schema.Number().Min(10).Optional();
        
        // Act
        var result = validator.Validate(15);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Optional_WithInvalidValue_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Number().Min(10).Optional();
        
        // Act
        var result = validator.Validate(5);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be greater than or equal to 10");
    }
} 