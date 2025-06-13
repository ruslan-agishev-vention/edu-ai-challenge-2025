using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Validator for array/collection values with support for length and element validation
/// </summary>
/// <typeparam name="T">The type of elements in the array</typeparam>
public class ArrayValidator<T> : BaseValidator<IEnumerable<T>>
{
    private readonly IValidator<T> _itemValidator;
    private int? _minLength;
    private int? _maxLength;
    private int? _exactLength;
    
    /// <summary>
    /// Initializes a new ArrayValidator with a validator for individual elements
    /// </summary>
    /// <param name="itemValidator">Validator to apply to each element in the array</param>
    public ArrayValidator(IValidator<T> itemValidator)
    {
        _itemValidator = itemValidator ?? throw new ArgumentNullException(nameof(itemValidator));
    }
    
    /// <summary>
    /// Sets the minimum required number of elements in the array
    /// </summary>
    /// <param name="minLength">Minimum number of elements</param>
    /// <returns>Current validator for method chaining</returns>
    public ArrayValidator<T> MinLength(int minLength)
    {
        _minLength = minLength;
        return this;
    }
    
    /// <summary>
    /// Sets the maximum allowed number of elements in the array
    /// </summary>
    /// <param name="maxLength">Maximum number of elements</param>
    /// <returns>Current validator for method chaining</returns>
    public ArrayValidator<T> MaxLength(int maxLength)
    {
        _maxLength = maxLength;
        return this;
    }
    
    /// <summary>
    /// Sets the exact required number of elements in the array
    /// </summary>
    /// <param name="exactLength">Exact number of elements required</param>
    /// <returns>Current validator for method chaining</returns>
    public ArrayValidator<T> ExactLength(int exactLength)
    {
        _exactLength = exactLength;
        return this;
    }
    
    /// <summary>
    /// Validates that the array is not empty
    /// </summary>
    /// <returns>Current validator for method chaining</returns>
    public ArrayValidator<T> NotEmpty()
    {
        _minLength = 1;
        return this;
    }
    
    /// <summary>
    /// Validates the array value against all configured rules
    /// </summary>
    /// <param name="value">The array value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(IEnumerable<T>? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Array cannot be null"));
        
        var errors = new List<string>();
        var items = value.ToList(); // Materialize to avoid multiple enumeration
        var length = items.Count;
        
        // Check exact length
        if (_exactLength.HasValue && length != _exactLength.Value)
            errors.Add(GetErrorMessage($"Array must contain exactly {_exactLength.Value} elements"));
        
        // Check minimum length (only if exact length is not specified)
        if (!_exactLength.HasValue && _minLength.HasValue && length < _minLength.Value)
            errors.Add(GetErrorMessage($"Array must contain at least {_minLength.Value} elements"));
        
        // Check maximum length (only if exact length is not specified)
        if (!_exactLength.HasValue && _maxLength.HasValue && length > _maxLength.Value)
            errors.Add(GetErrorMessage($"Array must contain no more than {_maxLength.Value} elements"));
        
        // Validate each element
        for (int i = 0; i < items.Count; i++)
        {
            var itemResult = _itemValidator.Validate(items[i]);
            if (!itemResult.IsValid)
            {
                foreach (var error in itemResult.Errors)
                {
                    errors.Add($"Element at index {i}: {error}");
                }
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
} 