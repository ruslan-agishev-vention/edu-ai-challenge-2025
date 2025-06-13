using FluentAssertions;
using Xunit;

namespace ValidationLibrary.Tests;

/// <summary>
/// Unit tests for Boolean, Date, and Number validators plus edge case testing
/// </summary>
public class AllValidatorTests
{
    #region BooleanValidator Tests
    
    [Fact]
    public void BooleanValidator_ValidBoolean_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Boolean();
        
        // Act
        var result = validator.Validate(true);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void BooleanValidator_NullValue_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Boolean();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be null");
    }
    
    [Fact]
    public void BooleanValidator_MustBeTrue_WithTrue_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Boolean().MustBeTrue();
        
        // Act
        var result = validator.Validate(true);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void BooleanValidator_MustBeTrue_WithFalse_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Boolean().MustBeTrue();
        
        // Act
        var result = validator.Validate(false);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be True");
    }
    
    [Fact]
    public void BooleanValidator_MustBeFalse_WithFalse_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Boolean().MustBeFalse();
        
        // Act
        var result = validator.Validate(false);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void BooleanValidator_MustBeFalse_WithTrue_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Boolean().MustBeFalse();
        
        // Act
        var result = validator.Validate(true);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value must be False");
    }
    
    #endregion
    
    #region DateValidator Tests
    
    [Fact]
    public void DateValidator_ValidDate_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Date();
        
        // Act
        var result = validator.Validate(DateTime.Now);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void DateValidator_NullValue_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Date();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Value cannot be null");
    }
    
    [Fact]
    public void DateValidator_MinDate_WithEarlierDate_ShouldReturnFailure()
    {
        // Arrange
        var minDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var validator = Schema.Date().MinDate(minDate);
        
        // Act
        var result = validator.Validate(new DateTime(2022, 12, 31, 0, 0, 0, DateTimeKind.Utc));
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Date must be after 2023-01-01");
    }
    
    [Fact]
    public void DateValidator_MaxDate_WithLaterDate_ShouldReturnFailure()
    {
        // Arrange
        var maxDate = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var validator = Schema.Date().MaxDate(maxDate);
        
        // Act
        var result = validator.Validate(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Date must be before 2023-12-31");
    }
    
    [Fact]
    public void DateValidator_InFuture_WithPastDate_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Date().InFuture();
        
        // Act
        var result = validator.Validate(DateTime.Now.AddDays(-1));
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Date failed custom validation");
    }
    
    [Fact]
    public void DateValidator_InPast_WithFutureDate_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Date().InPast();
        
        // Act
        var result = validator.Validate(DateTime.Now.AddDays(1));
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Date failed custom validation");
    }
    
    [Fact]
    public void DateValidator_IsToday_WithToday_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Date().IsToday();
        
        // Act
        var result = validator.Validate(DateTime.Today);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void DateValidator_IsWeekday_WithWeekday_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Date().IsWeekday();
        var monday = GetNextWeekday(DayOfWeek.Monday);
        
        // Act
        var result = validator.Validate(monday);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void DateValidator_IsWeekday_WithWeekend_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Date().IsWeekday();
        var saturday = GetNextWeekday(DayOfWeek.Saturday);
        
        // Act
        var result = validator.Validate(saturday);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Date failed custom validation");
    }
    
    #endregion
    
    #region NumberValidator Tests (Additional)
    
    [Fact]
    public void NumberValidator_ValidNumber_ShouldReturnSuccess()
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
    public void NumberValidator_NullNumber_ShouldReturnFailure()
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
    public void NumberValidator_Integer_WithWholeNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Integer();
        
        // Act
        var result = validator.Validate(42);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void NumberValidator_Integer_WithDecimalNumber_ShouldReturnFailure()
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
    public void NumberValidator_Positive_WithPositiveNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Positive();
        
        // Act
        var result = validator.Validate(5.5);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void NumberValidator_Negative_WithNegativeNumber_ShouldReturnSuccess()
    {
        // Arrange
        var validator = Schema.Number().Negative();
        
        // Act
        var result = validator.Validate(-5.5);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    #endregion
    
    #region Schema Factory Tests
    
    [Fact]
    public void Schema_String_ShouldReturnStringValidator()
    {
        // Act
        var validator = Schema.String();
        
        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeOfType<ValidationLibrary.Validators.StringValidator>();
    }
    
    [Fact]
    public void Schema_Number_ShouldReturnNumberValidator()
    {
        // Act
        var validator = Schema.Number();
        
        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeOfType<ValidationLibrary.Validators.NumberValidator>();
    }
    
    [Fact]
    public void Schema_Boolean_ShouldReturnBooleanValidator()
    {
        // Act
        var validator = Schema.Boolean();
        
        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeOfType<ValidationLibrary.Validators.BooleanValidator>();
    }
    
    [Fact]
    public void Schema_Date_ShouldReturnDateValidator()
    {
        // Act
        var validator = Schema.Date();
        
        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeOfType<ValidationLibrary.Validators.DateValidator>();
    }
    
    [Fact]
    public void Schema_Array_ShouldReturnArrayValidator()
    {
        // Act
        var validator = Schema.Array(Schema.String());
        
        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeOfType<ValidationLibrary.Validators.ArrayValidator<object>>();
    }
    
    [Fact]
    public void Schema_Any_ShouldReturnAnyValidator()
    {
        // Act
        var validator = Schema.Any<string>();
        
        // Assert
        validator.Should().NotBeNull();
        var result = validator.Validate("anything");
        result.IsValid.Should().BeTrue();
    }
    
    #endregion
    
    #region Edge Cases and Error Handling
    
    [Fact]
    public void ArrayValidator_WithNullArray_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Array(Schema.String());
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Array cannot be null");
    }
    
    [Fact]
    public void ArrayValidator_WithEmptyArray_ShouldWork()
    {
        // Arrange
        var validator = Schema.Array(Schema.String());
        
        // Act
        var result = validator.Validate(Array.Empty<string>());
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ArrayValidator_ExactLength_ShouldWork()
    {
        // Arrange
        var validator = Schema.Array(Schema.String()).ExactLength(3);
        
        // Act
        var result1 = validator.Validate(new[] { "a", "b", "c" });
        var result2 = validator.Validate(new[] { "a", "b" });
        
        // Assert
        result1.IsValid.Should().BeTrue();
        result2.IsValid.Should().BeFalse();
        result2.Errors.Should().Contain("Array must contain exactly 3 elements");
    }
    
    [Fact]
    public void ObjectValidator_WithNullObject_ShouldReturnFailure()
    {
        // Arrange
        var validator = Schema.Object<ComplexValidationTests.TestUser>();
        
        // Act
        var result = validator.Validate(null);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Object cannot be null");
    }
    
    [Fact]
    public void WithMessage_ShouldOverrideDefaultMessage()
    {
        // Arrange
        var customMessage = "Custom validation error";
        var validator = Schema.String().MinLength(10).WithMessage(customMessage);
        
        // Act
        var result = validator.Validate("short");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(customMessage);
        result.Errors.Should().NotContain("String must be at least 10 characters long");
    }
    
    [Fact]
    public void OptionalValidator_ChainedMethods_ShouldWork()
    {
        // Arrange
        var validator = Schema.String().MinLength(5).Optional().WithMessage("Custom message");
        
        // Act
        var result1 = validator.Validate(null);
        var result2 = validator.Validate("Hi");
        
        // Assert
        result1.IsValid.Should().BeTrue();
        result2.IsValid.Should().BeFalse();
        result2.Errors.Should().Contain("Custom message");
    }
    
    #endregion
    
    private static DateTime GetNextWeekday(DayOfWeek targetDay)
    {
        var today = DateTime.Today;
        var daysUntilTarget = ((int)targetDay - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilTarget == 0 && today.DayOfWeek != targetDay)
            daysUntilTarget = 7;
        return today.AddDays(daysUntilTarget);
    }
} 