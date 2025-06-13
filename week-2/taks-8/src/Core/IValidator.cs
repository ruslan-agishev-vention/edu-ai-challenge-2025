namespace ValidationLibrary.Core;

/// <summary>
/// Base interface for all validators, providing type-safe validation operations
/// </summary>
/// <typeparam name="T">The type being validated</typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the provided value and returns a validation result
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>A ValidationResult indicating success or failure with error messages</returns>
    ValidationResult Validate(T? value);
    
    /// <summary>
    /// Makes this validator optional, allowing null values to pass validation
    /// </summary>
    /// <returns>A new validator that treats null values as valid</returns>
    IValidator<T?> Optional();
    
    /// <summary>
    /// Sets a custom error message for this validator
    /// </summary>
    /// <param name="message">The custom error message</param>
    /// <returns>The current validator instance for method chaining</returns>
    IValidator<T> WithMessage(string message);
} 