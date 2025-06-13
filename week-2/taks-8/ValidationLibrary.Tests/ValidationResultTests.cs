using FluentAssertions;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests;

/// <summary>
/// Unit tests for ValidationResult class
/// </summary>
public class ValidationResultTests
{
    [Fact]
    public void Success_ShouldCreateValidResult()
    {
        // Act
        var result = ValidationResult.Success();
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Failure_WithSingleError_ShouldCreateInvalidResult()
    {
        // Arrange
        const string errorMessage = "Test error";
        
        // Act
        var result = ValidationResult.Failure(errorMessage);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(errorMessage);
    }
    
    [Fact]
    public void Failure_WithMultipleErrors_ShouldCreateInvalidResult()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        
        // Act
        var result = ValidationResult.Failure(errors);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void Combine_AllSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var results = new[]
        {
            ValidationResult.Success(),
            ValidationResult.Success(),
            ValidationResult.Success()
        };
        
        // Act
        var combined = ValidationResult.Combine(results);
        
        // Assert
        combined.IsValid.Should().BeTrue();
        combined.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Combine_SomeFailures_ShouldReturnFailure()
    {
        // Arrange
        var results = new[]
        {
            ValidationResult.Success(),
            ValidationResult.Failure("Error 1"),
            ValidationResult.Failure(new[] { "Error 2", "Error 3" })
        };
        
        // Act
        var combined = ValidationResult.Combine(results);
        
        // Assert
        combined.IsValid.Should().BeFalse();
        combined.Errors.Should().HaveCount(3);
        combined.Errors.Should().Contain("Error 1");
        combined.Errors.Should().Contain("Error 2");
        combined.Errors.Should().Contain("Error 3");
    }
    
    [Fact]
    public void Constructor_WithValidState_ShouldSetProperties()
    {
        // Arrange
        var errors = new[] { "Test error" };
        
        // Act
        var result = new ValidationResult(false, errors);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void Constructor_WithNullErrors_ShouldCreateEmptyErrorList()
    {
        // Act
        var result = new ValidationResult(true, null);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 