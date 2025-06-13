using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Validator for boolean values with support for required true/false validation
/// </summary>
public class BooleanValidator : BaseValidator<object?>
{
    private bool? _requiredValue;
    
    /// <summary>
    /// Validates that the boolean value must be true
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public BooleanValidator MustBeTrue()
    {
        _requiredValue = true;
        return this;
    }
    
    /// <summary>
    /// Validates that the boolean value must be false
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public BooleanValidator MustBeFalse()
    {
        _requiredValue = false;
        return this;
    }
    
    /// <summary>
    /// Converts various types to boolean for validation
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>Tuple indicating success and the converted value</returns>
    private static (bool success, bool value) TryConvertToBoolean(object? value)
    {
        if (value == null)
            return (false, false);
            
        try
        {
            return value switch
            {
                bool b => (true, b),
                string s when bool.TryParse(s, out var result) => (true, result),
                int i when i == 0 || i == 1 => (true, i == 1),
                _ => (false, false)
            };
        }
        catch
        {
            return (false, false);
        }
    }
    
    /// <summary>
    /// Validates the boolean value against all configured rules
    /// </summary>
    /// <param name="value">The boolean value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(object? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Value cannot be null"));
        
        var (success, boolValue) = TryConvertToBoolean(value);
        if (!success)
            return ValidationResult.Failure(GetErrorMessage($"Value must be a boolean type, but got {value.GetType().Name}"));
        
        if (_requiredValue.HasValue && boolValue != _requiredValue.Value)
        {
            return ValidationResult.Failure(GetErrorMessage($"Value must be {_requiredValue.Value}"));
        }
        
        return ValidationResult.Success();
    }
} 