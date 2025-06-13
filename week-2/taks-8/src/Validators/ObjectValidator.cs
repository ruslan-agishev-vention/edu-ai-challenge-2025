using System.Reflection;
using ValidationLibrary.Core;

namespace ValidationLibrary.Validators;

/// <summary>
/// Wrapper class to convert typed validators to object validators
/// </summary>
/// <typeparam name="T">The original type of the validator</typeparam>
internal class TypedValidatorWrapper<T> : IValidator<object?>
{
    private readonly IValidator<T> _innerValidator;

    public TypedValidatorWrapper(IValidator<T> innerValidator)
    {
        _innerValidator = innerValidator;
    }

    public ValidationResult Validate(object? value)
    {
        if (value == null)
        {
            return _innerValidator.Validate(default(T));
        }

        if (value is T typedValue)
        {
            return _innerValidator.Validate(typedValue);
        }

        return ValidationResult.Failure($"Expected type {typeof(T).Name} but got {value.GetType().Name}");
    }

    public IValidator<object?> Optional()
    {
        return new TypedValidatorWrapper<T>(_innerValidator.Optional() as IValidator<T> ?? _innerValidator);
    }

    public IValidator<object?> WithMessage(string message)
    {
        _innerValidator.WithMessage(message);
        return this;
    }
}

/// <summary>
/// Validator for object values with support for property-level validation
/// </summary>
/// <typeparam name="T">The type of object to validate</typeparam>
public class ObjectValidator<T> : BaseValidator<T> where T : class
{
    private readonly Dictionary<string, IValidator<object?>> _propertyValidators = new();
    private bool _allowAdditionalProperties = true;
    
    /// <summary>
    /// Initializes a new ObjectValidator with property validators
    /// </summary>
    /// <param name="propertyValidators">Dictionary mapping property names to their validators</param>
    public ObjectValidator(Dictionary<string, IValidator<object?>>? propertyValidators = null)
    {
        if (propertyValidators != null)
        {
            foreach (var (key, value) in propertyValidators)
            {
                _propertyValidators[key] = value;
            }
        }
    }
    
    /// <summary>
    /// Adds or updates a validator for a specific property (generic version)
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being validated</typeparam>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="validator">Validator for the property</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator<T> Property<TProperty>(string propertyName, IValidator<TProperty> validator)
    {
        _propertyValidators[propertyName] = new TypedValidatorWrapper<TProperty>(validator);
        return this;
    }
    
    /// <summary>
    /// Adds or updates a validator for a specific property
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="validator">Validator for the property</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator<T> Property(string propertyName, IValidator<object?> validator)
    {
        _propertyValidators[propertyName] = validator;
        return this;
    }
    
    /// <summary>
    /// Sets whether additional properties (not defined in validators) are allowed
    /// </summary>
    /// <param name="allow">Whether to allow additional properties</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator<T> AllowAdditionalProperties(bool allow = true)
    {
        _allowAdditionalProperties = allow;
        return this;
    }
    
    /// <summary>
    /// Validates the object value against all configured property rules
    /// </summary>
    /// <param name="value">The object value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(T? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Object cannot be null"));
        
        var errors = new List<string>();
        var objectType = value.GetType();
        var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var validatedProperties = new HashSet<string>();
        
        // Validate defined properties
        foreach (var (propertyName, validator) in _propertyValidators)
        {
            var property = properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                errors.Add($"Property '{propertyName}' not found on object");
                continue;
            }
            
            var propertyValue = property.GetValue(value);
            var result = validator.Validate(propertyValue);
            validatedProperties.Add(property.Name);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errors.Add($"Property '{propertyName}': {error}");
                }
            }
        }
        
        // Check for additional properties if not allowed
        if (!_allowAdditionalProperties)
        {
            var additionalProperties = properties
                .Where(p => !validatedProperties.Contains(p.Name))
                .Select(p => p.Name)
                .ToList();
            
            if (additionalProperties.Any())
            {
                errors.Add($"Additional properties not allowed: {string.Join(", ", additionalProperties)}");
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// Non-generic object validator for dynamic object validation
/// </summary>
public class ObjectValidator : BaseValidator<object>
{
    private readonly Dictionary<string, IValidator<object?>> _propertyValidators = new();
    private bool _allowAdditionalProperties = true;
    
    /// <summary>
    /// Initializes a new ObjectValidator with property validators
    /// </summary>
    /// <param name="propertyValidators">Dictionary mapping property names to their validators</param>
    public ObjectValidator(Dictionary<string, IValidator<object?>>? propertyValidators = null)
    {
        if (propertyValidators != null)
        {
            foreach (var (key, value) in propertyValidators)
            {
                _propertyValidators[key] = value;
            }
        }
    }
    
    /// <summary>
    /// Adds or updates a validator for a specific property (generic version)
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being validated</typeparam>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="validator">Validator for the property</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator Property<TProperty>(string propertyName, IValidator<TProperty> validator)
    {
        _propertyValidators[propertyName] = new TypedValidatorWrapper<TProperty>(validator);
        return this;
    }
    
    /// <summary>
    /// Adds or updates a validator for a specific property
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="validator">Validator for the property</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator Property(string propertyName, IValidator<object?> validator)
    {
        _propertyValidators[propertyName] = validator;
        return this;
    }
    
    /// <summary>
    /// Sets whether additional properties (not defined in validators) are allowed
    /// </summary>
    /// <param name="allow">Whether to allow additional properties</param>
    /// <returns>Current validator for method chaining</returns>
    public ObjectValidator AllowAdditionalProperties(bool allow = true)
    {
        _allowAdditionalProperties = allow;
        return this;
    }
    
    /// <summary>
    /// Validates the object value against all configured property rules
    /// </summary>
    /// <param name="value">The object value to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public override ValidationResult Validate(object? value)
    {
        if (value == null)
            return ValidationResult.Failure(GetErrorMessage("Object cannot be null"));
        
        var errors = new List<string>();
        var objectType = value.GetType();
        var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var validatedProperties = new HashSet<string>();
        
        // Validate defined properties
        foreach (var (propertyName, validator) in _propertyValidators)
        {
            var property = properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                errors.Add($"Property '{propertyName}' not found on object");
                continue;
            }
            
            var propertyValue = property.GetValue(value);
            var result = validator.Validate(propertyValue);
            validatedProperties.Add(property.Name);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errors.Add($"Property '{propertyName}': {error}");
                }
            }
        }
        
        // Check for additional properties if not allowed
        if (!_allowAdditionalProperties)
        {
            var additionalProperties = properties
                .Where(p => !validatedProperties.Contains(p.Name))
                .Select(p => p.Name)
                .ToList();
            
            if (additionalProperties.Any())
            {
                errors.Add($"Additional properties not allowed: {string.Join(", ", additionalProperties)}");
            }
        }
        
        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
} 