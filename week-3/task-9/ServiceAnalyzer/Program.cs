using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace ServiceAnalyzer;

static class Program
{
    private const string OpenAiChatApiEndpoint = "https://api.openai.com/v1/chat/completions";

    private const string SystemPrompt = @"You are a professional service analyst who creates comprehensive markdown reports about digital services and products. 

Your task is to analyze the provided service information and generate a well-structured markdown report with the following exact sections:

## Brief History
## Target Audience  
## Core Features
## Unique Selling Points
## Business Model
## Tech Stack Insights
## Perceived Strengths
## Perceived Weaknesses

Guidelines:
- Use clear, professional language
- Each section should contain 2-4 bullet points or short paragraphs
- Focus on factual information and reasonable inferences
- If analyzing a well-known service, use your knowledge about it
- If analyzing raw text, extract and infer relevant details
- Keep the tone neutral and analytical
- Use proper markdown formatting
- Ensure all 8 sections are included in your response";

    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 Service Analyzer");
        Console.WriteLine("==================");
        Console.WriteLine();

        // Load configuration
        var configuration = LoadConfiguration();
        var apiKey = GetApiKey(configuration);

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("❌ Error: OpenAI API key not found!");
            Console.WriteLine("Please set your API key using one of these methods:");
            Console.WriteLine("1. Set environment variable: OPENAI_API_KEY=your_key_here");
            Console.WriteLine("2. Create appsettings.json with OpenAI:ApiKey");
            return;
        }

        // Get user input
        Console.WriteLine("Enter service information:");
        Console.WriteLine("📝 Option 1: Service name (e.g., 'Spotify', 'Notion')");
        Console.WriteLine("📝 Option 2: Service description text");
        Console.WriteLine();
        Console.Write("Input: ");
        
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("❌ No input provided. Exiting...");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("🤖 Analyzing service... Please wait...");
        Console.WriteLine();        try
        {
            using var httpClient = new HttpClient();
            var report = await GenerateServiceReport(httpClient, apiKey, input);
            
            Console.WriteLine("📊 Service Analysis Report");
            Console.WriteLine("==========================");
            Console.WriteLine();
            Console.WriteLine(report);

            // Optionally save to file
            Console.WriteLine();
            Console.Write("💾 Save report to file? (y/n): ");
            var saveChoice = Console.ReadLine()?.ToLower();
            if (saveChoice == "y" || saveChoice == "yes")
            {
                var fileName = $"service_report_{DateTime.Now:yyyyMMdd_HHmmss}.md";
                await File.WriteAllTextAsync(fileName, report);
                Console.WriteLine($"✅ Report saved to: {fileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating report: {ex.Message}");
        }
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    private static string? GetApiKey(IConfiguration configuration)
    {
        return configuration["OPENAI_API_KEY"] ?? 
               configuration["OpenAI:ApiKey"] ?? 
               Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    }
    
    private static async Task<string> GenerateServiceReport(HttpClient httpClient, string apiKey, string input)
    {
        var prompt = CreateAnalysisPrompt(input);
          // List of models to try in order of preference
        var modelsToTry = new[] { "gpt-4.1-mini", "whisper-1" };
        
        foreach (var model in modelsToTry)
        {
            try
            {
                var requestBody = new
                {
                    model,
                    messages = new[]
                    {
                        new { role = "system", content = SystemPrompt },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 2000,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await httpClient.PostAsync(OpenAiChatApiEndpoint, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    Console.WriteLine($"✅ Successfully used model: {model}");
                    
                    return responseObject
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? "No response generated";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine($"⚠️  Model {model} not accessible, trying next...");
                    continue; // Try next model
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error with model {model}: {response.StatusCode} - {errorContent}");
                    continue; // Try next model
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception with model {model}: {ex.Message}");
            }
        }
        
        throw new InvalidOperationException("No available models could process the request. Please check your API key permissions.");
    }

    private static string CreateAnalysisPrompt(string input)
    {
        return $@"Please analyze the following service information and create a comprehensive report:

{input}

Generate a markdown-formatted report covering all the required sections. If this is a well-known service name, use your knowledge about it. If it's a description, extract and analyze the provided information while making reasonable inferences for missing details.";
    }
}
