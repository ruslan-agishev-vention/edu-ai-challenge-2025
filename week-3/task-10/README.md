# AI-Powered Product Filter

A C# .NET 8 console application that uses OpenAI's function calling to filter products based on natural language queries.

## Features

- 🤖 **OpenAI Function Calling**: Leverages OpenAI's GPT-4.1-mini to interpret natural language queries and convert them to structured filtering criteria
- 🛍️ **Product Search**: Searches through a JSON database of products across multiple categories (Electronics, Fitness, Kitchen, Books, Clothing)
- 💬 **Natural Language Interface**: Accept user queries like "I need electronics under $100 that are in stock"
- 📊 **Flexible Filtering**: Supports filtering by category, price range, rating, stock status, and search terms
- 🔒 **Secure Configuration**: Uses configuration files and environment variables for API key management

## Technology Stack

- **.NET 8.0**: Latest .NET framework
- **OpenAI API**: GPT-4.1-mini with function calling capabilities
- **System.Text.Json**: For JSON serialization/deserialization
- **Microsoft.Extensions.Configuration**: For configuration management

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- OpenAI API key (get one from [OpenAI Platform](https://platform.openai.com/))

## Installation & Setup

1. **Clone or download this project**
   ```bash
   git clone <repository-url>
   cd week-3/task-10
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Set up your OpenAI API key**

   **Option A: Using appsettings.json (recommended for development)**
   - Create `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-your-actual-api-key-here"
     }
   }
   ```

   **Option B: Using environment variable (recommended for production)**
   ```bash
   # Windows PowerShell
   $env:OpenAI__ApiKey = "sk-your-actual-api-key-here"
   
   # Windows Command Prompt
   set OpenAI__ApiKey=sk-your-actual-api-key-here
   
   # Linux/macOS
   export OpenAI__ApiKey="sk-your-actual-api-key-here"
   ```

4. **Build the application**
   ```bash
   dotnet build
   ```

## Running the Application

1. **Start the application**
   ```bash
   dotnet run
   ```

2. **Use the application**
   - The application will display a welcome message and load the product database
   - Enter natural language queries when prompted
   - Type 'quit' to exit the application

## Example Usage

### Sample Queries

- `"I need electronics under $100 that are in stock"`
- `"Show me fitness equipment with good ratings"`
- `"Find books under $30"`
- `"I want a smartphone with great camera and long battery life"`
- `"Show me kitchen appliances over $50 with rating above 4.5"`

### Expected Output Format

```
Filtered Products:
=====================
1. Wireless Headphones - $99.99, Rating: 4.5, In Stock
2. Smart Watch - $199.99, Rating: 4.6, In Stock

✅ Found 2 matching products.
```

## Project Structure

```
ProductFilterApp/
├── Models/
│   ├── Product.cs              # Product data model
│   └── FilterCriteria.cs       # Filter criteria model
├── Services/
│   └── OpenAIService.cs        # OpenAI integration service
├── Program.cs                  # Main application entry point
├── ProductFilterApp.csproj     # Project file
├── appsettings.json           # Configuration file
├── products.json              # Product database
└── README.md                  # This file
```

## How It Works

1. **Data Loading**: The application loads product data from `products.json`
2. **User Input**: User enters a natural language query
3. **OpenAI Processing**: The query is sent to OpenAI GPT-4 with a function definition
4. **Function Calling**: OpenAI analyzes the query and calls the `filter_products` function with structured parameters
5. **Filtering**: The application filters the product database based on the extracted criteria
6. **Results**: Matching products are displayed in a formatted list

## Function Calling Schema

The application defines a `filter_products` function with the following parameters:

- `category`: Product category (Electronics, Fitness, Kitchen, Books, Clothing)
- `max_price`: Maximum price filter
- `min_price`: Minimum price filter  
- `min_rating`: Minimum rating filter (0-5 scale)
- `in_stock_only`: Boolean to filter only in-stock items
- `search_term`: Text search within product names

## Error Handling

The application includes comprehensive error handling for:
- Missing or invalid OpenAI API key
- Network connectivity issues
- Invalid JSON data
- File not found errors
- OpenAI API errors

## Security Notes

- ⚠️ **Never commit your API key to version control**
- Use environment variables for production deployments
- The `appsettings.json` file should be added to `.gitignore` if it contains sensitive data

## Troubleshooting

### Common Issues

1. **"OpenAI API key not found"**
   - Ensure your API key is properly set in `appsettings.json` or environment variables
   - Check that the key starts with `sk-`

2. **"products.json file not found"**
   - Make sure `products.json` is in the same directory as the executable
   - Check that the file was copied to the output directory during build

3. **Network/API errors**
   - Verify your internet connection
   - Check if your OpenAI API key has sufficient credits
   - Ensure your API key has the correct permissions

### Debug Mode

To run in debug mode with more detailed output:
```bash
dotnet run --configuration Debug
```

## License

This project is for educational purposes as part of the AI Challenge 2025.
