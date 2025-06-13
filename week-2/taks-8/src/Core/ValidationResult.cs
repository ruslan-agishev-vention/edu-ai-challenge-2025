namespace ValidationLibrary.Core;

/// <summary>
/// Represents the result of a validation operation, containing success status and any error messages
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful
    /// </summary>
    public bool IsValid { get; }
    
    /// <summary>
    /// Gets the collection of error messages if validation failed
    /// </summary>
    public IReadOnlyList<string> Errors { get; }
    
    /// <summary>
    /// Initializes a new instance of ValidationResult with success status and optional errors
    /// </summary>
    /// <param name="isValid">Whether the validation was successful</param>
    /// <param name="errors">Collection of error messages</param>
    public ValidationResult(bool isValid, IEnumerable<string>? errors = null)
    {
        IsValid = isValid;
        Errors = errors?.ToList() ?? new List<string>();
    }
    
    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success() => new(true);
    
    /// <summary>
    /// Creates a failed validation result with a single error message
    /// </summary>
    /// <param name="error">The error message</param>
    public static ValidationResult Failure(string error) => new(false, new[] { error });
    
    /// <summary>
    /// Creates a failed validation result with multiple error messages
    /// </summary>
    /// <param name="errors">Collection of error messages</param>
    public static ValidationResult Failure(IEnumerable<string> errors) => new(false, errors);
    
    /// <summary>
    /// Combines multiple validation results into a single result
    /// </summary>
    /// <param name="results">Validation results to combine</param>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var allErrors = results.SelectMany(r => r.Errors).ToList();
        var isValid = results.All(r => r.IsValid);
        return new ValidationResult(isValid, allErrors);
    }
} 