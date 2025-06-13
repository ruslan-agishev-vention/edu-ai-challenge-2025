using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Validator for numeric values with support for range validation and comparisons
/// </summary>
public class NumberValidator : BaseValidator<object?>
{
    private double? _min;
    private double? _max;
    private bool _isMinInclusive = true;
    private bool _isMaxInclusive = true;
    private readonly List<Func<double, bool>> _customValidators = new();
    
    /// <summary>
    /// Sets the minimum allowed value (inclusive by default)
    /// </summary>
    /// <param name="min">Minimum value</param>
    /// <param name="inclusive">Whether the minimum is inclusive</param>
    /// <returns>Current validator for method chaining</returns>
    public NumberValidator Min(double min, bool inclusive = true)
    {
        _min = min;
        _isMinInclusive = inclusive;
        return this;
    }
    
    /// <summary>
    /// Sets the maximum allowed value (inclusive by default)
    /// </summary>
    /// <param name="max">Maximum value</param>
    /// <param name="inclusive">Whether the maximum is inclusive</param>
    /// <returns>Current validator for method chaining</returns>
    public NumberValidator Max(double max, bool inclusive = true)
    {
        _max = max;
        _isMaxInclusive = inclusive;
        return this;
    }
    
    /// <summary>
    /// Validates that the number is positive (greater than 0)
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public NumberValidator Positive()
    {
        _customValidators.Add(n => n > 0);
        return this;
    }
    
    /// <summary>
    /// Validates that the number is negative (less than 0)
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public NumberValidator Negative()
    {
        _customValidators.Add(n => n < 0);
        return this;
    }
    
    /// <summary>
    /// Validates that the number is an integer (no decimal places)
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public NumberValidator Integer()
    {
        _customValidators.Add(n => Math.Abs(n % 1) < double.Epsilon);
        return this;
    }
    
    /// <summary>
    /// Converts various numeric types to double for validation
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>Tuple indicating success and the converted value</returns>
    private static (bool success, double value) TryConvertToDouble(object? value)
    {
        if (value == null)
            return (false, 0);
            
        try
        {
            return value switch
            {
                double d => (true, d),
                float f => (true, f),
                decimal dec => (true, (double)dec),
                int i => (true, i),
                long l => (true, l),
                short s => (true, s),
                byte b => (true, b),
                sbyte sb => (true, sb),
                uint ui => (true, ui),
                ulong ul => (true, ul),
                ushort us => (true, us),
                _ => (false, 0)
            };
        }
        catch
        {
            return (false, 0);
        }
    }
    
    /// <summary>
    /// Validates the numeric value against all configured rules
    /// </summary>
    /// <param name="value">The numeric value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(object? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Value cannot be null"));
        
        var (success, numValue) = TryConvertToDouble(value);
        if (!success)
            return ValidationResult.Failure(GetErrorMessage($"Value must be a numeric type, but got {value.GetType().Name}"));
        
        var errors = new List<string>();
        
        // Check minimum value
        if (_min.HasValue)
        {
            if (_isMinInclusive && numValue < _min.Value)
                errors.Add(GetErrorMessage($"Value must be greater than or equal to {_min.Value}"));
            else if (!_isMinInclusive && numValue <= _min.Value)
                errors.Add(GetErrorMessage($"Value must be greater than {_min.Value}"));
        }
        
        // Check maximum value
        if (_max.HasValue)
        {
            if (_isMaxInclusive && numValue > _max.Value)
                errors.Add(GetErrorMessage($"Value must be less than or equal to {_max.Value}"));
            else if (!_isMaxInclusive && numValue >= _max.Value)
                errors.Add(GetErrorMessage($"Value must be less than {_max.Value}"));
        }
        
        // Check for NaN and infinity
        if (double.IsNaN(numValue))
            errors.Add(GetErrorMessage("Value cannot be NaN"));
        if (double.IsInfinity(numValue))
            errors.Add(GetErrorMessage("Value cannot be infinite"));
        
        // Check custom validators
        foreach (var validator in _customValidators)
        {
            if (!validator(numValue))
            {
                errors.Add(GetErrorMessage("Number failed custom validation"));
                break; // Only report first custom validation failure
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
} 