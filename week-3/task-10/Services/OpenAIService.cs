using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using ProductFilterApp.Models;
using System.Text.Json;

namespace ProductFilterApp.Services;

public class OpenAIService
{
    private readonly ChatClient _chatClient;
    private readonly List<Product> _products;

    public OpenAIService(IConfiguration configuration, List<Product> products)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OpenAI API key not found in configuration. Please set it in appsettings.json or environment variable.");
        }
        
        var openAIClient = new OpenAIClient(apiKey);
        _chatClient = openAIClient.GetChatClient("gpt-4.1-mini");
        _products = products;
    }

    public async Task<List<Product>> FilterProductsAsync(string userQuery)
    {
        // Define the function for filtering products
        var functionDefinition = ChatTool.CreateFunctionTool(
            functionName: "filter_products",
            functionDescription: "Filter products based on user criteria such as category, price range, rating, stock status, and search terms.",
            functionParameters: BinaryData.FromString("""
            {
                
                "type": "object",
                "properties": {
                    "category": {
                        "type": "string",
                        "description": "Product category to filter by (Electronics, Fitness, Kitchen, Books, Clothing)",
                        "enum": ["Electronics", "Fitness", "Kitchen", "Books", "Clothing"]
                    },
                    "max_price": {
                        "type": "number",
                        "description": "Maximum price for products"
                    },
                    "min_price": {
                        "type": "number",
                        "description": "Minimum price for products"
                    },
                    "min_rating": {
                        "type": "number",
                        "description": "Minimum rating for products (0-5 scale)"
                    },
                    "in_stock_only": {
                        "type": "boolean",
                        "description": "Whether to only show products that are in stock"
                    },
                    "search_term": {
                        "type": "string",
                        "description": "Search term to match against product names"
                    }
                },
                "additionalProperties": false
            }
            """)
        );

        // Prepare the system message with the product data
        var systemMessage = $"""
            You are a product search assistant. You have access to a product database and can filter products based on user preferences.
            When a user provides search criteria, analyze their request and call the filter_products function with appropriate parameters.
            Be flexible in interpreting user requests - if they mention specific product types, brands, or features, use the search_term parameter.
            If they mention price constraints, use max_price and/or min_price.
            If they want good quality items, set a reasonable min_rating (e.g., 4.0 for "good" products).
            If they mention availability, set in_stock_only to true.
            """;

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemMessage),
            new UserChatMessage(userQuery)
        };

        var options = new ChatCompletionOptions
        {
            Tools = { functionDefinition }
        };

        var response = await _chatClient.CompleteChatAsync(messages, options);

        // Check if the model wants to call the function
        if (response.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            foreach (var toolCall in response.Value.ToolCalls)
            {
                if (toolCall.FunctionName == "filter_products")
                {
                    var filterCriteria = JsonSerializer.Deserialize<FilterCriteria>(toolCall.FunctionArguments);
                    return FilterProducts(filterCriteria);
                }
            }
        }

        // If no function call was made, return empty list
        return new List<Product>();
    }

    private List<Product> FilterProducts(FilterCriteria? criteria)
    {
        if (criteria == null)
            return _products;

        var filteredProducts = _products.AsEnumerable();

        // Filter by category
        if (!string.IsNullOrEmpty(criteria.Category))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.Category.Equals(criteria.Category, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by price range
        if (criteria.MaxPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price <= criteria.MaxPrice.Value);
        }

        if (criteria.MinPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price >= criteria.MinPrice.Value);
        }

        // Filter by minimum rating
        if (criteria.MinRating.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Rating >= criteria.MinRating.Value);
        }

        // Filter by stock status
        if (criteria.InStockOnly == true)
        {
            filteredProducts = filteredProducts.Where(p => p.InStock);
        }

        // Filter by search term
        if (!string.IsNullOrEmpty(criteria.SearchTerm))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.Name.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return filteredProducts.ToList();
    }
}
