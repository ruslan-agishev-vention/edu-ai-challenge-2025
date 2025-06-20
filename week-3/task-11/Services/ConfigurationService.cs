using Microsoft.Extensions.Configuration;

namespace AudioTranscriber.Services;

public class ConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public string GetOpenAIApiKey()
    {
        string? apiKey = _configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key not found. Please set it using one of these methods:\n" +
                "1. Add to appsettings.json: \"OpenAI\": { \"ApiKey\": \"your-api-key\" }\n" +
                "2. Environment Variable: set OPENAI_API_KEY=your-api-key");
        }

        return apiKey;
    }
}
