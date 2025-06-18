using Microsoft.Extensions.Configuration;
using ProductFilterApp.Models;
using ProductFilterApp.Services;
using System.Text.Json;

namespace ProductFilterApp;

static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🛍️  AI-Powered Product Filter");
        Console.WriteLine("===============================");
        Console.WriteLine();

        try
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Load products from JSON file
            var products = await LoadProductsAsync();
            Console.WriteLine($"📦 Loaded {products.Count} products from database");
            Console.WriteLine();

            // Initialize OpenAI service
            var openAIService = new OpenAIService(configuration, products);

            // Main application loop
            while (true)
            {
                Console.WriteLine("💬 Enter your product search request (or 'quit' to exit):");
                Console.WriteLine("   Examples:");
                Console.WriteLine("   - \"I need electronics under $100 that are in stock\"");
                Console.WriteLine("   - \"Show me fitness equipment with good ratings\"");
                Console.WriteLine("   - \"Find books under $30\"");
                Console.WriteLine();
                Console.Write("🔍 Your request: ");

                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("❌ Please enter a valid search request.\n");
                    continue;
                }

                if (userInput.Trim().ToLower() == "quit")
                {
                    Console.WriteLine("👋 Goodbye!");
                    break;
                }

                Console.WriteLine("\n🤖 Processing your request with AI...");

                try
                {
                    var filteredProducts = await openAIService.FilterProductsAsync(userInput);

                    Console.WriteLine();
                    Console.WriteLine("📋 Filtered Products:");
                    Console.WriteLine("=====================");

                    if (filteredProducts.Count == 0)
                    {
                        Console.WriteLine("❌ No products found matching your criteria.");
                    }
                    else
                    {
                        for (int i = 0; i < filteredProducts.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {filteredProducts[i]}");
                        }
                    }

                    Console.WriteLine($"\n✅ Found {filteredProducts.Count} matching products.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing request: {ex.Message}");
                }

                Console.WriteLine("\n" + new string('-', 50) + "\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Application error: {ex.Message}");
            Console.WriteLine("\n💡 Make sure you have:");
            Console.WriteLine("   1. Set your OpenAI API key in appsettings.json or environment variable");
            Console.WriteLine("   2. Installed all required NuGet packages");
            Console.WriteLine("   3. Have the products.json file in the output directory");
        }
    }

    private static async Task<List<Product>> LoadProductsAsync()
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync("products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(jsonContent);
            return products ?? new List<Product>();
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException("products.json file not found. Make sure it exists in the output directory.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Error parsing products.json: {ex.Message}");
        }
    }
}
