using ValidationLibrary.Core;
using ValidationLibrary.Validators;

namespace ValidationLibrary;

/// <summary>
/// Main entry point for creating validators. Provides factory methods for all validator types.
/// </summary>
public static class Schema
{
    /// <summary>
    /// Creates a new string validator for validating string values
    /// </summary>
    /// <returns>A new StringValidator instance</returns>
    /// <example>
    /// var validator = Schema.String().MinLength(5).MaxLength(50);
    /// var result = validator.Validate("Hello World");
    /// </example>
    public static StringValidator String()
    {
        return new StringValidator();
    }
    
    /// <summary>
    /// Creates a new number validator for validating numeric values
    /// </summary>
    /// <returns>A new NumberValidator instance</returns>
    /// <example>
    /// var validator = Schema.Number().Min(0).Max(100);
    /// var result = validator.Validate(42.5);
    /// </example>
    public static NumberValidator Number()
    {
        return new NumberValidator();
    }
    
    /// <summary>
    /// Creates a new boolean validator for validating boolean values
    /// </summary>
    /// <returns>A new BooleanValidator instance</returns>
    /// <example>
    /// var validator = Schema.Boolean().MustBeTrue();
    /// var result = validator.Validate(true);
    /// </example>
    public static BooleanValidator Boolean()
    {
        return new BooleanValidator();
    }
    
    /// <summary>
    /// Creates a new date validator for validating DateTime values
    /// </summary>
    /// <returns>A new DateValidator instance</returns>
    /// <example>
    /// var validator = Schema.Date().InFuture();
    /// var result = validator.Validate(DateTime.Now.AddDays(1));
    /// </example>
    public static DateValidator Date()
    {
        return new DateValidator();
    }
    
    /// <summary>
    /// Creates a new array validator for validating collections with element validation
    /// </summary>
    /// <typeparam name="T">The type of elements in the array</typeparam>
    /// <param name="itemValidator">Validator to apply to each element</param>
    /// <returns>A new ArrayValidator instance</returns>
    /// <example>
    /// var validator = Schema.Array(Schema.String().MinLength(1));
    /// var result = validator.Validate(new[] { "hello", "world" });
    /// </example>
    public static ArrayValidator<T> Array<T>(IValidator<T> itemValidator)
    {
        return new ArrayValidator<T>(itemValidator);
    }
    
    /// <summary>
    /// Creates a new object validator for validating objects with property-level validation
    /// </summary>
    /// <typeparam name="T">The type of object to validate</typeparam>
    /// <param name="propertyValidators">Dictionary mapping property names to validators</param>
    /// <returns>A new ObjectValidator instance</returns>
    /// <example>
    /// var validator = Schema.Object&lt;User&gt;()
    ///     .Property("Name", Schema.String().MinLength(2))
    ///     .Property("Age", Schema.Number().Min(0));
    /// </example>
    public static ObjectValidator<T> Object<T>(Dictionary<string, IValidator<object?>>? propertyValidators = null) where T : class
    {
        return new ObjectValidator<T>(propertyValidators);
    }
    
    /// <summary>
    /// Creates a new object validator for validating dynamic objects
    /// </summary>
    /// <param name="propertyValidators">Dictionary mapping property names to validators</param>
    /// <returns>A new ObjectValidator instance</returns>
    /// <example>
    /// var validator = Schema.Object()
    ///     .Property("Name", Schema.String().MinLength(2))
    ///     .Property("Age", Schema.Number().Min(0));
    /// </example>
    public static ObjectValidator Object(Dictionary<string, IValidator<object?>>? propertyValidators = null)
    {
        return new ObjectValidator(propertyValidators);
    }
    
    /// <summary>
    /// Creates a validator that always passes validation (useful for optional fields)
    /// </summary>
    /// <typeparam name="T">The type being validated</typeparam>
    /// <returns>A validator that always returns success</returns>
    public static IValidator<T> Any<T>()
    {
        return new AnyValidator<T>();
    }
}

/// <summary>
/// A validator that accepts any value (always passes validation)
/// </summary>
/// <typeparam name="T">The type being validated</typeparam>
internal class AnyValidator<T> : BaseValidator<T>
{
    /// <summary>
    /// Validates any value (always returns success)
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>Always returns a successful ValidationResult</returns>
    public override ValidationResult Validate(T? value)
    {
        return ValidationResult.Success();
    }
} 