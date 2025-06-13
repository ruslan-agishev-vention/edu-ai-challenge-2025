using System.Text.RegularExpressions;
using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Validator for string values with support for length, pattern, and content validation
/// </summary>
public class StringValidator : BaseValidator<object?>
{
    private int? _minLength;
    private int? _maxLength;
    private Regex? _pattern;
    private readonly List<Func<string, bool>> _customValidators = new();
    
    /// <summary>
    /// Sets the minimum required length for the string
    /// </summary>
    /// <param name="minLength">Minimum length (inclusive)</param>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator MinLength(int minLength)
    {
        _minLength = minLength;
        return this;
    }
    
    /// <summary>
    /// Sets the maximum allowed length for the string
    /// </summary>
    /// <param name="maxLength">Maximum length (inclusive)</param>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator MaxLength(int maxLength)
    {
        _maxLength = maxLength;
        return this;
    }
    
    /// <summary>
    /// Adds a regex pattern that the string must match
    /// </summary>
    /// <param name="pattern">Regular expression pattern</param>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator Pattern(Regex pattern)
    {
        _pattern = pattern;
        return this;
    }
    
    /// <summary>
    /// Adds a regex pattern that the string must match
    /// </summary>
    /// <param name="pattern">Regular expression pattern as string</param>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator Pattern(string pattern)
    {
        _pattern = new Regex(pattern);
        return this;
    }
    
    /// <summary>
    /// Validates that the string is not empty or whitespace
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator NotEmpty()
    {
        _customValidators.Add(s => !string.IsNullOrWhiteSpace(s));
        return this;
    }
    
    /// <summary>
    /// Validates that the string contains only letters and digits
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public StringValidator AlphaNumeric()
    {
        _customValidators.Add(s => s.All(char.IsLetterOrDigit));
        return this;
    }
    
    /// <summary>
    /// Converts various types to string for validation
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>Tuple indicating success and the converted value</returns>
    private static (bool success, string value) TryConvertToString(object? value)
    {
        if (value == null)
            return (false, "");
            
        try
        {
            return value switch
            {
                string s => (true, s),
                char c => (true, c.ToString()),
                _ => (true, value.ToString() ?? "")
            };
        }
        catch
        {
            return (false, "");
        }
    }
    
    /// <summary>
    /// Validates the string value against all configured rules
    /// </summary>
    /// <param name="value">The string value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(object? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Value cannot be null"));
        
        var (success, stringValue) = TryConvertToString(value);
        if (!success)
            return ValidationResult.Failure(GetErrorMessage($"Value must be convertible to string, but got {value.GetType().Name}"));
        
        var errors = new List<string>();
        
        // Check minimum length
        if (_minLength.HasValue && stringValue.Length < _minLength.Value)
            errors.Add(GetErrorMessage($"String must be at least {_minLength.Value} characters long"));
        
        // Check maximum length
        if (_maxLength.HasValue && stringValue.Length > _maxLength.Value)
            errors.Add(GetErrorMessage($"String must be no more than {_maxLength.Value} characters long"));
        
        // Check pattern match
        if (_pattern != null && !_pattern.IsMatch(stringValue))
            errors.Add(GetErrorMessage($"String does not match required pattern: {_pattern}"));
        
        // Check custom validators
        foreach (var validator in _customValidators)
        {
            if (!validator(stringValue))
            {
                errors.Add(GetErrorMessage("String failed custom validation"));
                break; // Only report first custom validation failure
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
} 