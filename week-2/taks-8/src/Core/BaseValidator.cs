namespace ValidationLibrary.Core;

/// <summary>
/// Abstract base class for all validators, providing common validation functionality
/// </summary>
/// <typeparam name="T">The type being validated</typeparam>
public abstract class BaseValidator<T> : IValidator<T>
{
    protected string? CustomErrorMessage { get; private set; }
    
    /// <summary>
    /// Validates the provided value and returns a validation result
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>A ValidationResult indicating success or failure with error messages</returns>
    public abstract ValidationResult Validate(T? value);
    
    /// <summary>
    /// Makes this validator optional, allowing null values to pass validation
    /// </summary>
    /// <returns>A new validator that treats null values as valid</returns>
    public virtual IValidator<T?> Optional()
    {
        return new OptionalValidator<T>(this);
    }
    
    /// <summary>
    /// Sets a custom error message for this validator
    /// </summary>
    /// <param name="message">The custom error message</param>
    /// <returns>The current validator instance for method chaining</returns>
    public virtual IValidator<T> WithMessage(string message)
    {
        CustomErrorMessage = message;
        return this;
    }
    
    /// <summary>
    /// Creates an error message, using custom message if available, otherwise the default
    /// </summary>
    /// <param name="defaultMessage">The default error message</param>
    /// <returns>The error message to use</returns>
    protected string GetErrorMessage(string defaultMessage)
    {
        return CustomErrorMessage ?? defaultMessage;
    }
}

/// <summary>
/// Wrapper validator that makes any validator optional by allowing null values
/// </summary>
/// <typeparam name="T">The type being validated</typeparam>
internal class OptionalValidator<T> : IValidator<T?>
{
    private readonly IValidator<T> _innerValidator;
    
    public OptionalValidator(IValidator<T> innerValidator)
    {
        _innerValidator = innerValidator;
    }
    
    /// <summary>
    /// Validates the value, returning success if null, otherwise delegates to inner validator
    /// </summary>
    public ValidationResult Validate(T? value)
    {
        if (value == null)
            return ValidationResult.Success();
            
        return _innerValidator.Validate(value);
    }
    
    /// <summary>
    /// Returns self since this validator is already optional
    /// </summary>
    public IValidator<T?> Optional() => this;
    
    /// <summary>
    /// Sets custom error message on the inner validator
    /// </summary>
    public IValidator<T?> WithMessage(string message)
    {
        _innerValidator.WithMessage(message);
        return this;
    }
} 