using System.Globalization;
using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Validator for date/time values with support for range and temporal validation
/// </summary>
public class DateValidator : BaseValidator<object?>
{
    private DateTime? _minDate;
    private DateTime? _maxDate;
    private readonly List<Func<DateTime, bool>> _customValidators = new();
    
    /// <summary>
    /// Sets the minimum allowed date
    /// </summary>
    /// <param name="minDate">Minimum date</param>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator MinDate(DateTime minDate)
    {
        _minDate = minDate;
        return this;
    }
    
    /// <summary>
    /// Sets the maximum allowed date
    /// </summary>
    /// <param name="maxDate">Maximum date</param>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator MaxDate(DateTime maxDate)
    {
        _maxDate = maxDate;
        return this;
    }
    
    /// <summary>
    /// Validates that the date is in the future
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator InFuture()
    {
        _customValidators.Add(d => d > DateTime.Now);
        return this;
    }
    
    /// <summary>
    /// Validates that the date is in the past
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator InPast()
    {
        _customValidators.Add(d => d < DateTime.Now);
        return this;
    }
    
    /// <summary>
    /// Validates that the date is today
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator IsToday()
    {
        _customValidators.Add(d => d.Date == DateTime.Today);
        return this;
    }
    
    /// <summary>
    /// Validates that the date is a weekday (Monday-Friday)
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public DateValidator IsWeekday()
    {
        _customValidators.Add(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
        return this;
    }
    
    /// <summary>
    /// Converts various types to DateTime for validation
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>Tuple indicating success and the converted value</returns>
    private static (bool success, DateTime value) TryConvertToDateTime(object? value)
    {
        if (value == null)
            return (false, DateTime.MinValue);
            
        try
        {
            return value switch
            {
                DateTime dt => (true, dt),
                DateTimeOffset dto => (true, dto.DateTime),
                DateOnly dateOnly => (true, dateOnly.ToDateTime(TimeOnly.MinValue)),
                string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, out var result) => (true, result),
                _ => (false, DateTime.MinValue)
            };
        }
        catch
        {
            return (false, DateTime.MinValue);
        }
    }
    
    /// <summary>
    /// Validates the date value against all configured rules
    /// </summary>
    /// <param name="value">The date value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(object? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Value cannot be null"));
        
        var (success, dateValue) = TryConvertToDateTime(value);
        if (!success)
            return ValidationResult.Failure(GetErrorMessage($"Value must be a date/time type, but got {value.GetType().Name}"));
        
        var errors = new List<string>();
        
        // Check minimum date
        if (_minDate.HasValue && dateValue < _minDate.Value)
            errors.Add(GetErrorMessage($"Date must be after {_minDate.Value:yyyy-MM-dd}"));
        
        // Check maximum date
        if (_maxDate.HasValue && dateValue > _maxDate.Value)
            errors.Add(GetErrorMessage($"Date must be before {_maxDate.Value:yyyy-MM-dd}"));
        
        // Check custom validators
        foreach (var validator in _customValidators)
        {
            if (!validator(dateValue))
            {
                errors.Add(GetErrorMessage("Date failed custom validation"));
                break; // Only report first custom validation failure
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
} 