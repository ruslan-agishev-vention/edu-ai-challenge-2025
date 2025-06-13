# ValidationLibrary

A robust, type-safe validation library for C# that provides comprehensive validation for complex data structures including primitives, arrays, and objects.

## Features

- **Type-safe validation** with full C# type system integration
- **Fluent API** for method chaining and readable validation rules
- **Comprehensive validators** for primitives (string, number, boolean, date)
- **Complex type support** for arrays and objects with nested validation
- **Custom error messages** with easy customization
- **Optional validation** with null-value handling
- **Extensive test coverage** ensuring reliability and robustness

## Installation

### Prerequisites

- .NET 8.0 or later
- Visual Studio 2022 or compatible IDE (optional but recommended)

### Building from Source

1. Clone or download the repository to your local machine
2. Open a terminal/command prompt in the project directory
3. Build the library:
   ```bash
   dotnet build ValidationLibrary.csproj
   ```

## Quick Start

### Basic Usage

```csharp
using ValidationLibrary;

// Create validators using the Schema factory
var stringValidator = Schema.String().MinLength(5).MaxLength(50);
var result = stringValidator.Validate("Hello World");

if (result.IsValid)
{
    Console.WriteLine("Validation passed!");
}
else
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Error: {error}");
    }
}
```

### Primitive Type Validation

#### String Validation
```csharp
var validator = Schema.String()
    .MinLength(5)
    .MaxLength(100)
    .Pattern(@"^[A-Za-z\s]+$")  // Only letters and spaces
    .NotEmpty()
    .WithMessage("Name must be 5-100 characters, letters only");

var result = validator.Validate("John Doe");
```

#### Number Validation
```csharp
var validator = Schema.Number()
    .Min(0)
    .Max(120)
    .Integer()
    .Positive()
    .WithMessage("Age must be a positive integer between 0 and 120");

var result = validator.Validate(25);
```

#### Boolean Validation
```csharp
var validator = Schema.Boolean()
    .MustBeTrue()
    .WithMessage("You must accept the terms and conditions");

var result = validator.Validate(true);
```

#### Date Validation
```csharp
var validator = Schema.Date()
    .MinDate(DateTime.Now)
    .InFuture()
    .IsWeekday()
    .WithMessage("Date must be a future weekday");

var result = validator.Validate(DateTime.Now.AddDays(1));
```

### Array Validation

```csharp
// Validate array of strings
var validator = Schema.Array(Schema.String().MinLength(2))
    .MinLength(1)
    .MaxLength(10)
    .NotEmpty();

var tags = new[] { "development", "design", "testing" };
var result = validator.Validate(tags);
```

### Object Validation

#### Simple Object Validation
```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public bool IsActive { get; set; }
}

var validator = Schema.Object<User>()
    .Property("Name", Schema.String().MinLength(2).MaxLength(50))
    .Property("Email", Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    .Property("Age", Schema.Number().Min(0).Max(150).Integer())
    .Property("IsActive", Schema.Boolean());

var user = new User
{
    Name = "John Doe",
    Email = "john@example.com",
    Age = 30,
    IsActive = true
};

var result = validator.Validate(user);
```

#### Complex Nested Object Validation
```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public List<string> Tags { get; set; }
    public Address Address { get; set; }
}

// Create nested validators
var addressValidator = Schema.Object<Address>()
    .Property("Street", Schema.String().MinLength(5))
    .Property("City", Schema.String().MinLength(2))
    .Property("PostalCode", Schema.String().Pattern(@"^\d{5}$"))
    .Property("Country", Schema.String().MinLength(2));

var userValidator = Schema.Object<User>()
    .Property("Name", Schema.String().MinLength(2).MaxLength(50))
    .Property("Email", Schema.String().Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    .Property("Age", Schema.Number().Min(18).Max(150).Integer())
    .Property("Tags", Schema.Array(Schema.String().MinLength(1)).MinLength(1))
    .Property("Address", addressValidator);

var user = new User
{
    Name = "John Doe",
    Email = "john@example.com",
    Age = 30,
    Tags = new List<string> { "developer", "designer" },
    Address = new Address
    {
        Street = "123 Main St",
        City = "Anytown",
        PostalCode = "12345",
        Country = "USA"
    }
};

var result = userValidator.Validate(user);
```

### Optional Validation

Make any validator optional to allow null values:

```csharp
var validator = Schema.String().MinLength(5).Optional();

// These will both pass
var result1 = validator.Validate(null);        // Success
var result2 = validator.Validate("Hello");     // Success
var result3 = validator.Validate("Hi");        // Failure - too short
```

### Custom Error Messages

Customize error messages for better user experience:

```csharp
var validator = Schema.String()
    .MinLength(8)
    .Pattern(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])")
    .WithMessage("Password must be at least 8 characters with uppercase, lowercase, number, and special character");
```

### Real-World Examples

#### Email Validation
```csharp
var emailValidator = Schema.String()
    .Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
    .WithMessage("Please enter a valid email address");
```

#### Phone Number Validation
```csharp
var phoneValidator = Schema.String()
    .Pattern(@"^\+?1?[-.\s]?\(?[0-9]{3}\)?[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$")
    .WithMessage("Please enter a valid phone number");
```

#### Password Validation
```csharp
var passwordValidator = Schema.String()
    .MinLength(8)
    .MaxLength(100)
    .Pattern(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])")
    .WithMessage("Password must be 8-100 characters with at least one uppercase letter, lowercase letter, number, and special character");
```

## API Reference

### Schema Factory Methods

- `Schema.String()` - Creates a string validator
- `Schema.Number()` - Creates a number validator  
- `Schema.Boolean()` - Creates a boolean validator
- `Schema.Date()` - Creates a date validator
- `Schema.Array<T>(IValidator<T>)` - Creates an array validator
- `Schema.Object<T>()` - Creates an object validator
- `Schema.Any<T>()` - Creates a validator that accepts any value

### String Validator Methods

- `MinLength(int)` - Sets minimum string length
- `MaxLength(int)` - Sets maximum string length
- `Pattern(string|Regex)` - Sets regex pattern requirement
- `NotEmpty()` - Requires non-empty, non-whitespace string
- `AlphaNumeric()` - Requires only letters and digits

### Number Validator Methods

- `Min(double, bool inclusive = true)` - Sets minimum value
- `Max(double, bool inclusive = true)` - Sets maximum value
- `Positive()` - Requires positive number (> 0)
- `Negative()` - Requires negative number (< 0)
- `Integer()` - Requires whole number (no decimals)

### Boolean Validator Methods

- `MustBeTrue()` - Requires value to be true
- `MustBeFalse()` - Requires value to be false

### Date Validator Methods

- `MinDate(DateTime)` - Sets minimum date
- `MaxDate(DateTime)` - Sets maximum date
- `InFuture()` - Requires future date
- `InPast()` - Requires past date
- `IsToday()` - Requires today's date
- `IsWeekday()` - Requires weekday (Monday-Friday)

### Array Validator Methods

- `MinLength(int)` - Sets minimum array length
- `MaxLength(int)` - Sets maximum array length
- `ExactLength(int)` - Sets exact required length
- `NotEmpty()` - Requires at least one element

### Object Validator Methods

- `Property(string, IValidator<object?>)` - Adds property validator
- `AllowAdditionalProperties(bool)` - Controls additional property validation

### Common Methods (All Validators)

- `Optional()` - Makes validator optional (allows null)
- `WithMessage(string)` - Sets custom error message
- `Validate(T?)` - Validates value and returns result

## Running Tests

To run the unit tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Test Coverage

The library includes comprehensive test coverage for:

- All primitive validators (string, number, boolean, date)
- Array validation with various scenarios
- Object validation including nested objects
- Optional validation behavior
- Custom error messages
- Complex real-world validation scenarios
- Edge cases and error conditions

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For questions, issues, or feature requests, please create an issue in the project repository. 